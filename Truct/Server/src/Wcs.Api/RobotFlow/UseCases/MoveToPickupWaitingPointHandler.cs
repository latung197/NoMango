using Wcs.Rcs.Contracts;

using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Rcs.DTOs;
using Wcs.Common.ValueObjects;
using Wcs.Common.Events;
using Wcs.Api.Configs;
using Wcs.Api.RobotFlow;
using Wcs.Api.Services;
using Microsoft.Extensions.Options;
using Wcs.Rcs;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class MoveToPickupWaitingPointHandler(
  ILogger<MoveToPickupWaitingPointHandler> logger, IRcsClient rcsClient, IEventPublisher publisher, ITaskStateStore store, IOptions<HikConfig> options, IWmsService wmsService)
{
  private readonly ILogger<MoveToPickupWaitingPointHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly HikConfig _hikConfig = options.Value;
  private readonly IWmsService _wmsService = wmsService;

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
    var station = flowTask.FromStation;
    if (station.HasWarehouseDoor) {
      var cranePosition = flowTask.CranePositions?.FirstOrDefault()
        ?? throw new InvalidOperationException("Crane position is required for warehouse outbound");
      var positionCode = $"F{cranePosition.Floor}-R{cranePosition.Row}-P{cranePosition.Position}";
      _logger.LogInformation(
        "[MoveToPickupWaitingPoint] TaskId={TaskId} — WMS ConfirmPositionOutbound, Position={PositionCode}",
        flowTask.Id, positionCode);
      try {
        await _wmsService.ConfirmPositionOutboundAsync(positionCode, ct);
      } catch (WmsApiException ex) {
        _logger.LogWarning(
          ex,
          "[MoveToPickupWaitingPoint] WMS confirm failed. TaskId={TaskId}, Position={PositionCode}, StatusCode={StatusCode}",
          flowTask.Id, positionCode, ex.StatusCode);
        await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, $"ConfirmPositionOutbound (position={positionCode})", ex, ct);
        await _publisher.PublishAsync(
          new BizError(flowTask.Id.ToString(), "WMS_004", "WMS từ chối xác nhận vị trí xuất kho"),
          ct);
        return;
      } catch (Exception ex) {
        _logger.LogError(ex, "[MoveToPickupWaitingPoint] WMS confirm failed. TaskId={TaskId}, Position={PositionCode}", flowTask.Id, positionCode);
        await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, $"ConfirmPositionOutbound (position={positionCode})", ex.Message, ct);
        await _publisher.PublishAsync(
          new BizError(flowTask.Id.ToString(), "WMS_004", "Lỗi gọi WMS xác nhận vị trí xuất kho"),
          ct);
        return;
      }
    }

    var taskTyp = _hikConfig.GetMainFlowTaskTyp(flowTask.TaskSize);
    var rcsTask = await _rcsClient.GenAgvSchedulingTask(new CreateTaskRequest(
      TaskTyp: taskTyp,
      Priority: "1",
      PositionCodePath: [
        RcsPositions.Berth(flowTask.FromStation.WaitingPoint),
      ],
      AgvCode: flowTask.Robot?.Code ?? null
    ), ct);

    if (!rcsTask.Success) {
      await FlowTaskApiErrorLogger.LogRcsAsync(_store, flowTask, "GenAgvSchedulingTask", rcsTask.Code, rcsTask.Message, ct);
      await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "RCS001", "Failed to create RCS task"), ct);
      throw new InvalidDataException("Failed to create RCS task");
    }

    flowTask.RcsTaskId = rcsTask.Data;

    flowTask.UpdateStep(FlowStep.MoveToPickupWaitingPoint);
    await _store.SaveAsync(flowTask);
  }
}