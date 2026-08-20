using System.Text.Json;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Extensions;
using Wcs.Common.Exceptions;
using Wcs.Common.ValueObjects;
using Wcs.Rcs.Contracts;
using Wcs.Rcs.DTOs;

namespace Wcs.Api.RobotFlow;

public class TaskStateStore(
    IFlowTaskRepository flowTaskRepository,
    IStationRepository stationRepository,
    StationService stationService,
    RobotService robotService,
    IRcsClient rcsClient,
    StationControlCleanupService stationControlCleanup,
    ILogger<TaskStateStore> logger
) : ITaskStateStore
{   
    private readonly IFlowTaskRepository _flowTaskRepository = flowTaskRepository;
    private readonly ILogger<TaskStateStore> _logger = logger;
    private readonly IStationRepository _stationRepository = stationRepository;
    private readonly StationService _stationService = stationService;
    private readonly RobotService _robotService = robotService;
    private readonly IRcsClient _rcsClient = rcsClient;
    private readonly StationControlCleanupService _stationControlCleanup = stationControlCleanup;
    public async Task<FlowTask?> LoadAsync(string taskId)
    {
        return await _flowTaskRepository.GetByIdAsync(taskId);
    }

    public Task SaveAsync(FlowTask task)
    {
        return _flowTaskRepository.UpdateAsync(task);
    }

    public async Task<FlowTask> CreateTask(string fromStation, string toStation, string? robotCode, List<CranePosition>? cranePositions = null, string? cassetteCode = null, string? storageStageCode = null, int? quantity = null, string? product = null, Size? taskSize = null)
    {
        var fromStationEntity = await _stationService.GetStationByCode(fromStation);
        var toStationEntity = await _stationService.GetStationByCode(toStation);

        if (fromStationEntity == null || toStationEntity == null)
        {
            throw new InvalidStationException("Station not found");
        }

        if (fromStationEntity.HasWarehouseDoor || toStationEntity.HasWarehouseDoor) {
            if (cranePositions == null || cranePositions.Count == 0) {
                throw new InvalidOperationException("Crane positions are required when warehouse door is present");
            }
        }

        var existingRouteTasks = await _flowTaskRepository.GetActiveTasksByRouteAsync(fromStation, toStation);
        if (existingRouteTasks.Any())
        {
            throw new StationNotAvailableException("There is already an active task for this route");
        }

        var existingTasks = await _flowTaskRepository.GetTasksInProgressOnStationAsync(fromStation);
        if (existingTasks.Any())
        {
            throw new StationNotAvailableException("There is already a task in progress on this station");
        }

        existingTasks = await _flowTaskRepository.GetTasksInProgressOnStationAsync(toStation);
        if (existingTasks.Any())
        {
            throw new StationNotAvailableException("There is already a task in progress on this station");
        }
        Robot? robotEntity = null;
        if (robotCode != null) {
            existingTasks = await _flowTaskRepository.GetTasksInProgressOnRobotAsync(robotCode);
            if (existingTasks.Any())
            {
                throw new RobotNotAvailableException("There is already a task in progress on this robot");
            }

            robotEntity = await _robotService.GetRobotByCode(robotCode) ?? throw new RobotNotAvailableException("Robot not found");
        }

        var resolvedTaskSize = taskSize ?? StationSizeResolver.ResolveForStation(fromStationEntity, null);

        FlowTask flowTask = new()
        {
            FromStation = fromStationEntity,
            ToStation = toStationEntity,
            Robot = robotEntity,
            CurrentStep = FlowStep.Initial,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CranePositions = cranePositions,
            CassetteCode = cassetteCode,
            StorageStageCode = storageStageCode,
            Quantity = quantity,
            Product = product,
            TaskSize = resolvedTaskSize
        };

        return await _flowTaskRepository.CreateAsync(flowTask);
    }

    public async Task<FlowTask> CreateFallbackTask(string toStation, string? robotCode, FlowStep startStep)
    {
        var toStationEntity = await _stationService.GetStationByCode(toStation) ?? throw new InvalidStationException("Station not found");

        var existingTasks = await _flowTaskRepository.GetTasksInProgressOnStationAsync(toStation);
        if (existingTasks.Any())
        {
            throw new StationNotAvailableException("There is already a task in progress on this station");
        }

        Robot? robotEntity = null;
        if (robotCode != null) {
            existingTasks = await _flowTaskRepository.GetTasksInProgressOnRobotAsync(robotCode);
            if (existingTasks.Any())
            {
                throw new RobotNotAvailableException("There is already a task in progress on this robot");
            }

            robotEntity = await _robotService.GetRobotByCode(robotCode) ?? throw new RobotNotAvailableException("Robot not found");
        }
        FlowTask flowTask = new()
        {
            ToStation = toStationEntity,
            Robot = robotEntity,
            CurrentStep = startStep,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _flowTaskRepository.CreateAsync(flowTask);
    }

    public Task LogAsync(FlowTask task, string logMessage, string? eventName = null)
    {
        return _flowTaskRepository.LogAsync(task, logMessage, eventName);
    }

    public async Task CancelTask(string taskId, bool isUserCancel = false)
    {
        var task = await _flowTaskRepository.GetByIdAsync(taskId) ?? throw new InvalidOperationException("Task not found");

        if (isUserCancel && task.FromStation is { HasWarehouseDoor: true })
        {
            throw new InvalidOperationException("Không thể hủy task xuất kho sau khi FlowTask đã được tạo");
        }

        if (task.RcsTaskId != null) {
            await _rcsClient.CancelTask(new CancelTaskRequest(task.RcsTaskId, "0"));
        }

        await _stationControlCleanup.ResetControlTagsForTaskAsync(task, "Flow task cancelled", CancellationToken.None);

        task.Cancel();
        await _flowTaskRepository.UpdateAsync(task);
    }
}
