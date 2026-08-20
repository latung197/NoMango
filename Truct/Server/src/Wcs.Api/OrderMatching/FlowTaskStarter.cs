using Wcs.Api.Services;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.OrderMatching;

public sealed class FlowTaskStarter(
    ITaskStateStore taskStateStore,
    IEventPublisher publisher,
    TaskExecutionGateService taskExecutionGate,
    StationService stationService,
    ILogger<FlowTaskStarter> logger)
{
    private readonly ITaskStateStore _taskStateStore = taskStateStore;
    private readonly IEventPublisher _publisher = publisher;
    private readonly TaskExecutionGateService _taskExecutionGate = taskExecutionGate;
    private readonly StationService _stationService = stationService;
    private readonly ILogger<FlowTaskStarter> _logger = logger;

    public async Task<FlowTask> StartStationToStationAsync(
        string fromStation,
        string toStation,
        string? robotCode,
        List<CranePosition>? cranePositions = null,
        string? cassetteCode = null,
        string? storageStageCode = null,
        int? quantity = null,
        string? product = null,
        Size? taskSize = null,
        CancellationToken ct = default)
    {
        var from = await _stationService.GetStationByCode(fromStation)
            ?? throw new InvalidOperationException($"Trạm {fromStation} không tồn tại");
        var to = await _stationService.GetStationByCode(toStation)
            ?? throw new InvalidOperationException($"Trạm {toStation} không tồn tại");

        await _stationService.ValidateStationsReadyForTaskAsync(from, to);
        await _stationService.ValidateStationCanAcceptDropAsync(to);

        var flowTask = await _taskStateStore.CreateTask(
            fromStation, toStation, robotCode, cranePositions,
            cassetteCode, storageStageCode, quantity, product, taskSize);
        await PublishStationChangesAndStartAsync(flowTask, ct);
        return flowTask;
    }

    private async Task PublishStationChangesAndStartAsync(FlowTask flowTask, CancellationToken ct)
    {
        await _publisher.PublishAsync(new StationChanged(flowTask.FromStation), ct);
        await _publisher.PublishAsync(new StationChanged(flowTask.ToStation), ct);
        await PublishFlowStartedIfAllowedAsync(flowTask, ct);
    }

    private async Task PublishFlowStartedIfAllowedAsync(FlowTask flowTask, CancellationToken ct)
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

        await _publisher.PublishAsync(new FlowStarted(flowTask.Id.ToString()), ct);
    }
}
