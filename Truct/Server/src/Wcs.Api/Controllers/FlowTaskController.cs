using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.Extensions;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Controllers;

[ApiController]
[Route("api/flow-tasks")]
public class FlowTaskController(
    IFlowTaskRepository flowTaskRepository,
    ILogger<FlowTaskController> logger,
    ITaskStateStore taskStateStore,
    IEventPublisher publisher,
    StationService stationService,
    ITaskEventBroadcaster eventBroadcaster,
    TaskExecutionGateService taskExecutionGate
    ) : ControllerBase
{
    private readonly IFlowTaskRepository _flowTaskRepository = flowTaskRepository;
    private readonly ILogger<FlowTaskController> _logger = logger;
    private readonly ITaskStateStore _taskStateStore = taskStateStore;
    private readonly IEventPublisher _publisher = publisher;
    private readonly StationService _stationService = stationService;
    private readonly ITaskEventBroadcaster _eventBroadcaster = eventBroadcaster;
    private readonly TaskExecutionGateService _taskExecutionGate = taskExecutionGate;

    private async Task PublishFlowStartedIfAllowedAsync(FlowTask flowTask, CancellationToken ct = default)
    {
        if (await _taskExecutionGate.IsPausedAsync(ct))
        {
            flowTask.Status = FlowStatus.Pending;
            await _taskStateStore.SaveAsync(flowTask);
            _logger.LogInformation(
                "Task {TaskId} tạo khi execution paused — giữ Pending, chờ resume",
                flowTask.Id);
            return;
        }

        await _publisher.PublishAsync(new FlowStarted(flowTask.Id.ToString()));
    }

    private async Task<ActionResult<BaseResponse<FlowTask>>?> ValidateStationsRunningAsync(params Station[] stations)
    {
        foreach (var station in stations)
        {
            var reason = await _stationService.GetStationNotRunningReasonAsync(station);
            if (reason != null)
            {
                return BadRequest(BaseResponse<FlowTask>.ErrorResult(reason, "STATION_NOT_RUNNING"));
            }
        }

        return null;
    }

    private async Task<ActionResult<BaseResponse<FlowTask>>?> ValidateStationCanAcceptDropAsync(Station station)
    {
        var reason = await _stationService.GetStationCannotAcceptDropReasonAsync(station);
        if (reason != null)
        {
            return BadRequest(BaseResponse<FlowTask>.ErrorResult(reason, "STATION_FULL"));
        }

        return null;
    }

    private static readonly JsonSerializerOptions _sseJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Lấy danh sách task đang active (dùng cho monitor UI)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowTask>>>> GetActiveTasks()
    {
        var tasks = await _flowTaskRepository.GetActiveTasksAsync();
        return Ok(BaseResponse<IEnumerable<FlowTask>>.SuccessResult(tasks, "Lấy danh sách task active thành công"));
    }

    /// <summary>
    /// Lấy danh sách task gần đây (mọi trạng thái, mới nhất trước)
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowTask>>>> GetRecentTasks([FromQuery] int limit = 30)
    {
        var tasks = await _flowTaskRepository.GetAllAsync();
        return Ok(BaseResponse<IEnumerable<FlowTask>>.SuccessResult(tasks.Take(limit), $"Lấy {Math.Min(limit, tasks.Count())} task gần đây thành công"));
    }

    /// <summary>
    /// Lấy danh sách task đã bị hủy (mới nhất trước)
    /// </summary>
    [HttpGet("cancelled")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowTask>>>> GetCancelledTasks([FromQuery] int limit = 50)
    {
        var tasks = await _flowTaskRepository.GetCancelledTasksAsync(limit);
        return Ok(BaseResponse<IEnumerable<FlowTask>>.SuccessResult(tasks, $"Lấy {tasks.Count()} task đã hủy thành công"));
    }

    /// <summary>
    /// Lấy flow task theo Task ID
    /// </summary>
    [HttpGet("by-task-id/{taskId}")]
    public async Task<ActionResult<BaseResponse<FlowTask>>> GetFlowTaskByTaskId(string taskId)
    {
        var flowTask = await _flowTaskRepository.GetByIdAsync(taskId);
        if (flowTask == null) {
            return NotFound(BaseResponse<FlowTask>.ErrorResult(
                $"FlowTask với Task ID {taskId} không tồn tại", 
                "FLOWTASK_NOT_FOUND"));
        }

        return Ok(BaseResponse<FlowTask>.SuccessResult(flowTask, "Lấy thông tin FlowTask thành công"));
    }

    /// <summary>
    /// Tạo flow task mang hàng từ trạm tới trạm khác
    /// </summary>
    [HttpPost("station-to-station")]
    public async Task<ActionResult<BaseResponse<FlowTask>>> CreateFlowTask(StationToStationTaskRequest request)
    {
        List<CranePosition>? cranePositions = null;
        if (request.CranePositions != null && request.CranePositions.Count > 0)
        {
            cranePositions = request.CranePositions.Select(cp => new CranePosition(cp.Row, cp.Floor, cp.Position)).ToList();
        }

        var fromStationEntity = await _stationService.GetStationByCode(request.FromStation);
        var toStationEntity = await _stationService.GetStationByCode(request.ToStation);
        if (fromStationEntity is null || toStationEntity is null)
        {
            return NotFound(BaseResponse<FlowTask>.ErrorResult("Trạm không tồn tại", "STATION_NOT_FOUND"));
        }

        Size taskSize;
        try
        {
            taskSize = StationSizeResolver.ResolveForStation(fromStationEntity, request.Size);
            if (!toStationEntity.SupportsSize(taskSize))
            {
                return BadRequest(BaseResponse<FlowTask>.ErrorResult(
                    $"Trạm đích {toStationEntity.Code} không hỗ trợ size {taskSize}",
                    "SIZE_MISMATCH"));
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<FlowTask>.ErrorResult(ex.Message, ex.Message == "SIZE_REQUIRED" ? "SIZE_REQUIRED" : "TRANSFER_REQUEST_ERROR"));
        }

        var runningCheck = await ValidateStationsRunningAsync(fromStationEntity, toStationEntity);
        if (runningCheck != null)
            return runningCheck;

        var dropCheck = await ValidateStationCanAcceptDropAsync(toStationEntity);
        if (dropCheck != null)
            return dropCheck;
        
        var flowTask = await _taskStateStore.CreateTask(
            request.FromStation, request.ToStation, request.RobotCode, cranePositions,
            request.CassetteCode, request.StorageStageCode, request.Quantity, request.Product, taskSize);
        await _publisher.PublishAsync(new StationChanged(flowTask.FromStation));
        await _publisher.PublishAsync(new StationChanged(flowTask.ToStation));
        await PublishFlowStartedIfAllowedAsync(flowTask);
        return Ok(BaseResponse<FlowTask>.SuccessResult(flowTask, "Tạo flow task thành công"));
    }

    /// <summary>
    /// Tạo flow task mang hàng từ stage tới station khác
    /// </summary>
    [HttpPost("stage-to-station")]
    public async Task<ActionResult<BaseResponse<FlowTask>>> CreateFlowTask(StageToStationTaskRequest request)
    {
        var toStation = await _stationService.GetStationByCode(request.ToStation);
        if (toStation == null) {
            return NotFound(BaseResponse<FlowTask>.ErrorResult(
                $"Trạm {request.ToStation} không tồn tại", 
                "STATION_NOT_FOUND"));
        }

        Size taskSize;
        try
        {
            taskSize = StationSizeResolver.ResolveForStation(toStation, request.Size);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<FlowTask>.ErrorResult(ex.Message, ex.Message == "SIZE_REQUIRED" ? "SIZE_REQUIRED" : "TRANSFER_REQUEST_ERROR"));
        }

        var fromStation = await _stationService.GetHasCassetteStationByStageAndSize(request.FromStage, taskSize.ToString());
        if (fromStation == null) {
            return NotFound(BaseResponse<FlowTask>.ErrorResult(
                $"Không tìm thấy trạm có cassette từ stage {request.FromStage} và size {taskSize}", 
                "STATION_NOT_FOUND"));
        }

        var runningCheck = await ValidateStationsRunningAsync(fromStation, toStation);
        if (runningCheck != null)
            return runningCheck;

        var dropCheck = await ValidateStationCanAcceptDropAsync(toStation);
        if (dropCheck != null)
            return dropCheck;

        List<CranePosition>? cranePositions = null;
        if (request.CranePositions != null && request.CranePositions.Count > 0)
        {
            cranePositions = request.CranePositions.Select(cp => new CranePosition(cp.Row, cp.Floor, cp.Position)).ToList();
        }

        var flowTask = await _taskStateStore.CreateTask(fromStation.Code, request.ToStation, request.RobotCode, cranePositions, request.CassetteCode, request.StorageStageCode, request.Quantity, request.Product, taskSize);
        await _publisher.PublishAsync(new StationChanged(flowTask.FromStation));
        await _publisher.PublishAsync(new StationChanged(flowTask.ToStation));
        await PublishFlowStartedIfAllowedAsync(flowTask);
        return Ok(BaseResponse<FlowTask>.SuccessResult(flowTask, "Tạo flow task thành công"));
    }

    /// <summary>
    /// Tạo flow task mang hàng từ station tới stage khác
    /// </summary>
    [HttpPost("station-to-stage")]
    public async Task<ActionResult<BaseResponse<FlowTask>>> CreateFlowTask(StationToStageTaskRequest request)
    {
        var fromStation = await _stationService.GetStationByCode(request.FromStation);
        if (fromStation == null) {
            return NotFound(BaseResponse<FlowTask>.ErrorResult(
                $"Trạm {request.FromStation} không tồn tại", 
                "STATION_NOT_FOUND"));
        }

        Size taskSize;
        try
        {
            taskSize = StationSizeResolver.ResolveForStation(fromStation, request.Size);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<FlowTask>.ErrorResult(ex.Message, ex.Message == "SIZE_REQUIRED" ? "SIZE_REQUIRED" : "TRANSFER_REQUEST_ERROR"));
        }

        var toStation = await _stationService.GetEmptyStationByStageAndSize(request.ToStage, taskSize.ToString());
        if (toStation == null) {
            return NotFound(BaseResponse<FlowTask>.ErrorResult(
                $"Không tìm thấy trạm trống từ stage {request.ToStage} và size {taskSize}", 
                "STATION_NOT_FOUND"));
        }

        var runningCheck = await ValidateStationsRunningAsync(fromStation, toStation);
        if (runningCheck != null)
            return runningCheck;

        List<CranePosition>? cranePositions = null;
        if (request.CranePositions != null && request.CranePositions.Count > 0)
        {
            cranePositions = request.CranePositions.Select(cp => new CranePosition(cp.Row, cp.Floor, cp.Position)).ToList();
        }

        var flowTask = await _taskStateStore.CreateTask(fromStation.Code, toStation.Code, request.RobotCode, cranePositions, request.CassetteCode, request.StorageStageCode, request.Quantity, request.Product, taskSize);
        await _publisher.PublishAsync(new StationChanged(flowTask.FromStation));
        await _publisher.PublishAsync(new StationChanged(flowTask.ToStation));
        await PublishFlowStartedIfAllowedAsync(flowTask);
        return Ok(BaseResponse<FlowTask>.SuccessResult(flowTask, "Tạo flow task thành công"));
    }

    /// <summary>
    /// Tạo fallback flow task
    /// </summary>
    [HttpPost("fallback")]
    public async Task<ActionResult<BaseResponse<FlowTask>>> CreateFallbackTask(CreateFallbackTaskRequest request)
    {
        var toStation = await _stationService.GetStationByCode(request.ToStation);
        if (toStation == null)
        {
            return NotFound(BaseResponse<FlowTask>.ErrorResult(
                $"Trạm {request.ToStation} không tồn tại",
                "STATION_NOT_FOUND"));
        }

        var runningCheck = await ValidateStationsRunningAsync(toStation);
        if (runningCheck != null)
            return runningCheck;

        var flowTask = await _taskStateStore.CreateFallbackTask(request.ToStation, request.RobotCode, request.StartStep);
        await PublishFlowStartedIfAllowedAsync(flowTask);
        return Ok(BaseResponse<FlowTask>.SuccessResult(flowTask, "Tạo fallback flow task thành công"));
    }

    /// <summary>
    /// Tiếp tục flow task
    /// </summary>
    [HttpPost("resume/{taskId}")]
    public async Task<ActionResult<BaseResponse<FlowTask>>> ResumeFlowTask(string taskId)
    {
        var flowTask = await _taskStateStore.LoadAsync(taskId);
        if (flowTask == null) {
            return NotFound(BaseResponse<FlowTask>.ErrorResult(
                $"FlowTask với Task ID {taskId} không tồn tại", 
                "FLOWTASK_NOT_FOUND"));
        }
        await _publisher.PublishAsync(new FlowResumed(flowTask.Id.ToString()));
        return Ok(BaseResponse<FlowTask>.SuccessResult(flowTask, "Tiếp tục flow task thành công"));
    }

    /// <summary>
    /// Cancel flow task. Task xuất kho (fromStation là cửa kho) không được hủy sau khi đã tạo.
    /// </summary>
    [HttpPost("cancel/{taskId}")]
    public async Task<ActionResult<BaseResponse<string>>> CancelFlowTask(string taskId)
    {
        var flowTask = await _taskStateStore.LoadAsync(taskId);
        if (flowTask == null)
        {
            return NotFound(BaseResponse<string>.ErrorResult(
                $"FlowTask với Task ID {taskId} không tồn tại",
                "FLOWTASK_NOT_FOUND"));
        }

        if (flowTask.FromStation is { HasWarehouseDoor: true })
        {
            return BadRequest(BaseResponse<string>.ErrorResult(
                "Không thể hủy task xuất kho sau khi FlowTask đã được tạo",
                "WAREHOUSE_OUTBOUND_CANCEL_FORBIDDEN"));
        }

        await _taskStateStore.CancelTask(taskId, isUserCancel: true);
        return Ok(BaseResponse<string>.SuccessResult("OK", "Hủy flow task thành công"));
    }

    /// <summary>
    /// Lấy lịch sử log của flow task (mặc định chỉ lấy các dòng lỗi)
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<FlowTaskHistoryDto>>>> GetTaskHistory(
        [FromQuery] string? taskId = null,
        [FromQuery] bool errorsOnly = true,
        [FromQuery(Name = "event")] string? eventName = null,
        [FromQuery] int limit = 100)
    {
        var entries = await _flowTaskRepository.GetHistoryAsync(taskId, errorsOnly, eventName, limit);
        var dtos = entries.Select(h => new FlowTaskHistoryDto(
            h.Id,
            h.FlowTaskId,
            h.Event,
            h.Step,
            h.LogMessage,
            h.CreatedAt
        )).ToList();

        return Ok(BaseResponse<IReadOnlyList<FlowTaskHistoryDto>>.SuccessResult(
            dtos,
            $"Lấy {dtos.Count} bản ghi lịch sử thành công"));
    }

    /// <summary>
    /// Server-Sent Events stream: push live events của task về UI theo thời gian thực.
    /// Không lưu DB — events chỉ tồn tại trong memory, mất khi browser reload.
    /// </summary>
    [HttpGet("{taskId}/stream")]
    public async Task StreamTaskEvents(string taskId, CancellationToken ct)
    {
        Response.Headers["Content-Type"]      = "text/event-stream";
        Response.Headers["Cache-Control"]     = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";
        Response.Headers["Access-Control-Allow-Origin"] = Request.Headers["Origin"].FirstOrDefault() ?? "*";

        // Gửi event kết nối thành công
        var connected = new TaskLiveEvent("Connected", taskId, "SSE stream opened", DateTime.UtcNow);
        await WriteSseAsync(connected, ct);

        await foreach (var evt in _eventBroadcaster.SubscribeAsync(taskId, ct))
        {
            await WriteSseAsync(evt, ct);
        }
    }

    private async Task WriteSseAsync(TaskLiveEvent evt, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(evt, _sseJson);
        await Response.WriteAsync($"data: {json}\n\n", ct);
        await Response.Body.FlushAsync(ct);
    }
}

