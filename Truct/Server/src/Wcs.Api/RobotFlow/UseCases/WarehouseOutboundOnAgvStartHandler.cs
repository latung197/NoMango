using Wcs.Api.RobotFlow;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Constants;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Okamura;
using Wcs.Okamura.Models;
using Wcs.Okamura.Services;
using CranePosition = Wcs.Okamura.Models.CranePosition;

namespace Wcs.Api.RobotFlow.UseCases;

/// <summary>
/// Gửi lệnh crane xuất kho khi AGV bắt đầu thực hiện leg 1 (MoveToPickupWaitingPoint).
/// Tránh kéo hàng ra trước khi AGV thực sự di chuyển.
/// </summary>
public sealed class WarehouseOutboundOnAgvStartHandler(
  ILogger<WarehouseOutboundOnAgvStartHandler> logger,
  ITaskStateStore store,
  ICraneService craneService,
  ICraneTaskNoGenerator craneTaskNoGenerator,
  IEventPublisher publisher,
  TaskScopedLockService taskLocks,
  CraneTaskDispatchService craneTaskDispatchService)
{
  private readonly ILogger<WarehouseOutboundOnAgvStartHandler> _logger = logger;
  private readonly ITaskStateStore _store = store;
  private readonly ICraneService _craneService = craneService;
  private readonly ICraneTaskNoGenerator _craneTaskNoGenerator = craneTaskNoGenerator;
  private readonly IEventPublisher _publisher = publisher;
  private readonly TaskScopedLockService _taskLocks = taskLocks;
  private readonly CraneTaskDispatchService _craneTaskDispatchService = craneTaskDispatchService;

  public async Task Handle(RobotTaskBegin @event, CancellationToken ct)
  {
    if (!HikCallbackStep.TryGetFlowStep(@event.Step, out var legStep)
        || legStep != FlowStep.MoveToPickupWaitingPoint)
    {
      return;
    }

    await using var taskLock = await _taskLocks.AcquireAsync($"crane-dispatch:{@event.TaskId}", ct);

    var flowTask = await _store.LoadAsync(@event.TaskId);
    if (flowTask is null)
    {
      _logger.LogWarning(
        "[WarehouseOutboundOnAgvStart] FlowTask không tồn tại. TaskId={TaskId}, Step={Step}",
        @event.TaskId, @event.Step);
      return;
    }

    if (!flowTask.FromStation.HasWarehouseDoor)
    {
      return;
    }

    if (flowTask.CraneTaskNo is not null)
    {
      _logger.LogInformation(
        "[WarehouseOutboundOnAgvStart] TaskId={TaskId} — crane outbound đã gửi (TaskNo={CraneTaskNo}), bỏ qua",
        flowTask.Id, flowTask.CraneTaskNo);
      return;
    }

    if (flowTask.CurrentStep != FlowStep.MoveToPickupWaitingPoint)
    {
      _logger.LogWarning(
        "[WarehouseOutboundOnAgvStart] TaskId={TaskId} — step hiện tại {CurrentStep}, không phải MoveToPickupWaitingPoint",
        flowTask.Id, flowTask.CurrentStep);
      return;
    }

    var cranePosition = flowTask.CranePositions?.FirstOrDefault()
      ?? throw new InvalidOperationException("Crane position is required");
    var craneTaskNo = await _craneTaskNoGenerator.NextAsync(ct);
    var idempotencyKey = $"flow:{flowTask.Id}:crane:outbound:on-agv-start";
    var reservation = await _craneTaskDispatchService.ReserveFlowTaskDispatchAsync(
      flowTask.Id,
      craneTaskNo,
      Wcs.Okamura.Enums.CraneTaskType.Outbound,
      idempotencyKey,
      ct);

    craneTaskNo = reservation.Dispatch.TaskNo;
    flowTask.CraneTaskNo = craneTaskNo;
    await _store.SaveAsync(flowTask);

    if (!reservation.Created)
    {
      _logger.LogInformation(
        "[WarehouseOutboundOnAgvStart] TaskId={TaskId} — dùng lại crane dispatch đã có. TaskNo={TaskNo}, Status={Status}",
        flowTask.Id,
        reservation.Dispatch.TaskNo,
        reservation.Dispatch.Status);
      return;
    }

    _logger.LogInformation(
      "[WarehouseOutboundOnAgvStart] TaskId={TaskId} — AGV bắt đầu leg {Step}, gửi crane outbound TaskNo={TaskNo}",
      flowTask.Id, @event.Step, craneTaskNo);

    var craneResult = await _craneService.SendOutboundAsync(new OutboundTask(
      craneTaskNo,
      new CranePosition(cranePosition.Row, cranePosition.Floor, cranePosition.Position),
      CraneStationPositions.Put), ct);

    if (!craneResult.Success)
    {
      await _craneTaskDispatchService.MarkFailedAsync(
        reservation.Dispatch.TaskNo,
        craneResult.ErrorMessage ?? "Crane outbound dispatch failed",
        craneResult.ErrorCode,
        ct);
      flowTask.CraneTaskNo = null;
      await _store.SaveAsync(flowTask);
      await FlowTaskApiErrorLogger.LogCraneAsync(_store, flowTask, "SendOutbound", craneResult, ct);
      await _publisher.PublishAsync(
        new BizError(flowTask.Id.ToString(), $"CRANE0001", craneResult.ErrorMessage ?? "Crane error"),
        ct);
      await _publisher.PublishAsync(
        new FlowErrorOccurred(flowTask.Id.ToString(), FlowStep.BeforePick, $"CRANE0001"),
        ct);
      return;
    }

    await _craneTaskDispatchService.MarkAcceptedAsync(reservation.Dispatch.TaskNo, ct);
  }
}
