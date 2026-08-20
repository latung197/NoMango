using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class FlowTaskController(FlowTaskService flowService, StationService stationService, ILogger<FlowTaskController> logger) : ControllerBase
{
    private readonly FlowTaskService _flowService = flowService;
    private readonly StationService _stationService = stationService;
    private readonly ILogger<FlowTaskController> _logger = logger;

    /// <summary>
    /// Hiển thị danh sách flow-task
    /// </summary>
    [HttpGet("flow-tasks")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowTaskResponse>>>> List([FromQuery] FlowTaskListRequest request)
    {
        var flowTask = await _flowService.GetListAsync(request);
        return Ok(BaseResponse<IEnumerable<FlowTaskResponse>>.SuccessResult(
            [.. flowTask.Select((item, index) => FlowTaskResponse.FromFlowTask(item, index))],
            "Lấy danh sách FlowTask thành công",
            new Meta { Total = flowTask.Count(), CurrentPage = request.PageNumber, Size = request.PageSize }
        ));
    }

    /// <summary>
    /// Lấy danh sách task đã bị hủy (dùng cho Task Monitor)
    /// </summary>
    [HttpGet("flow-tasks/cancelled")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowTaskResponse>>>> Cancelled([FromQuery] int limit = 50)
    {
        var flowTasks = await _flowService.GetCancelledTasksAsync(limit);
        return Ok(BaseResponse<IEnumerable<FlowTaskResponse>>.SuccessResult(
            [.. flowTasks.Select((item, index) => FlowTaskResponse.FromFlowTask(item, index))],
            $"Lấy {flowTasks.Count()} task đã hủy thành công"
        ));
    }

    /// <summary>
    /// Tìm kiếm danh sách flow-task theo khu vực
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("flow-tasks/search")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowTaskResponse>>>> Search(FlowSearchRequest request)
    {
        var flowTask = await _flowService.GetListAsync(request);
        var stations = await _stationService.GetAllAsync();

        var filteredFlowTask = new List<FlowTask>();
        foreach (var ft in flowTask)
        {
            //var fromStation = await _stationService.GetStationByCode(ft.FromStation.Code);
            //var toStation = await _stationService.GetStationByCode(ft.ToStation.Code);
            var fromStation = stations.FirstOrDefault(station => string.Equals(station.Code, ft.FromStation.Code));
            var toStation = stations.FirstOrDefault(station => string.Equals(station.Code, ft.ToStation.Code));
            if (string.Equals(fromStation?.StageCode, request.Stage, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(toStation?.StageCode, request.Stage, StringComparison.OrdinalIgnoreCase))
            {
                if (fromStation != null)
                {
                    ft.FromStation = fromStation;
                }
                if (toStation != null)
                {
                    ft.ToStation = toStation;
                }
                filteredFlowTask.Add(ft);
            }
        }
        return Ok(BaseResponse<IEnumerable<FlowTaskResponse>>.SuccessResult(
            [.. filteredFlowTask.Select((item, index) => FlowTaskResponse.FromFlowTask(item, index))],
            "Lấy danh sách FlowTask thành công",
            new Meta { Total = flowTask.Count() }
        ));
    }

    /// <summary>
    /// Hiển thị flow-task theo code
    /// </summary>
    /*[HttpGet("flow-tasks/{id}")]
    public async Task<ActionResult<BaseResponse<FlowTaskResponse>>> Show(string id)
    {
        var flowTask = await _flowService.ShowFlowTaskAsync(id);
        return Ok(BaseResponse<FlowTaskResponse>.SuccessResult(FlowTaskResponse.FromFlowTask(flowTask), "Lấy thông tin FlowTask thành công"));
    }*/
}
