using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Constants;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Api.RobotFlow;
using Wcs.Rcs;
using Wcs.Rcs.Contracts;
using Wcs.Rcs.DTOs;

namespace Wcs.Api.Services;

public sealed class DropStationRedirectService(
    ILogger<DropStationRedirectService> logger,
    StationService stationService,
    IFlowTaskRepository flowTaskRepository,
    IRcsClient rcsClient,
    ITaskStateStore store,
    IEventPublisher publisher,
    IWmsService wmsService)
{
    private readonly ILogger<DropStationRedirectService> _logger = logger;
    private readonly StationService _stationService = stationService;
    private readonly IFlowTaskRepository _flowTaskRepository = flowTaskRepository;
    private readonly IRcsClient _rcsClient = rcsClient;
    private readonly ITaskStateStore _store = store;
    private readonly IEventPublisher _publisher = publisher;
    private readonly IWmsService _wmsService = wmsService;

    /// <summary>
    /// Chuyển hướng AMR sang trạm fallback khi trạm đặt hiện tại không thể hạ hàng.
    /// Dùng slot buffer HIK (71): continueTask đưa AMR từ A.Waiting → B.Waiting mà không hủy task.
    /// </summary>
    public async Task<bool> TryRedirectAsync(FlowTask task, CancellationToken ct)
    {
        var originalStation = task.ToStation;
        var fallbackStations = await _stationService.GetFallbackStationsAsync(originalStation, task.TaskSize);

        foreach (var fallback in fallbackStations)
        {
            var inProgress = await _flowTaskRepository.GetTasksInProgressOnStationAsync(fallback.Code, ct);
            if (inProgress.Any())
            {
                continue;
            }

            var fallbackEntity = await _stationService.GetStationByCode(fallback.Code);
            if (fallbackEntity is null)
            {
                continue;
            }

            if (!await _stationService.IsStationAvailableForDropAsync(fallbackEntity))
            {
                _logger.LogDebug(
                    "Bỏ qua trạm fallback {StationCode}: trạm đang có hàng",
                    fallback.Code);
                continue;
            }

            if (fallbackEntity.HasWarehouseDoor && fallbackEntity.Type == InOut.IN)
            {
                var cranePositions = await _wmsService.TryFindEmptyInboundPositionsAsync(ct);
                if (cranePositions is null || cranePositions.Count == 0)
                {
                    _logger.LogWarning(
                        "Bỏ qua trạm fallback {StationCode}: WMS không còn ô nhập kho trống",
                        fallbackEntity.Code);
                    await FlowTaskApiErrorLogger.LogWmsAsync(
                        _store,
                        task,
                        $"FindEmptyInboundPositions (fallback={fallbackEntity.Code})",
                        "WMS không còn ô nhập kho trống",
                        ct);
                    continue;
                }

                task.CranePositions = cranePositions.ToList();
            }

            task.ToStation = fallbackEntity;

            // continueTask này tiêu thụ leg đệm (filler 71) để chở AMR A.Waiting → B.Waiting.
            var rcsResult = await _rcsClient.ContinueTask(new ContinueTaskRequest(
                task.RcsTaskId!,
                RcsPositions.Berth(fallbackEntity.WaitingPoint)
            ), ct);

            if (!rcsResult.Success)
            {
                _logger.LogWarning(
                    "Drop redirect ContinueTask failed. TaskId={TaskId}, From={FromStation}, To={ToStation}, RcsMessage={Message}",
                    task.Id, originalStation.Code, fallbackEntity.Code, rcsResult.Message);
                await FlowTaskApiErrorLogger.LogRcsAsync(
                    _store, task, $"ContinueTask (redirect {originalStation.Code}→{fallbackEntity.Code})",
                    rcsResult.Code, rcsResult.Message, ct);
                task.ToStation = originalStation;
                continue;
            }

            // Đã tiêu thụ 1 filler để relocate; khi AMR tới B.Waiting (71_Complete) → reroute branch
            // gọi lại BeforeDrop để chờ rèm tại B, rồi đi thẳng EnterDropPoint vì filler đã đủ.
            task.ConsumedDropFillers++;
            task.RerouteCount++;
            task.Reroute = new RerouteRequest(
                ExpectedHikLeg: HikCallbackStep.DropFillerLeg,
                ResumeStep: FlowStep.BeforeDrop);
            task.SetWaitingConditions(WaitingSet.For(nameof(RobotTaskComplete)));
            task.UpdateStep(FlowStep.BeforeDrop);

            await _store.SaveAsync(task);
            await _store.LogAsync(
                task,
                $"Redirect drop: {originalStation.Code} → {fallbackEntity.Code}",
                nameof(TimeoutFired));

            await _publisher.PublishAsync(new BizError(
                task.Id.ToString(),
                "E005",
                $"Trạm đặt {originalStation.Code} không thể hạ hàng, chuyển hàng tới {fallbackEntity.Code}"
            ), ct);

            _logger.LogInformation(
                "Drop redirect started. TaskId={TaskId}, From={FromStation}, To={ToStation}",
                task.Id, originalStation.Code, fallbackEntity.Code);
            return true;
        }

        return false;
    }
}
