using Wcs.Rcs.Contracts;

using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Api.RobotFlow.Flow;
using Wcs.Rcs.DTOs;
using Wcs.OpcUa.Contracts;
using Wcs.Okamura;
using Wcs.Okamura.Services;
using Wcs.Okamura.Models;
using Wcs.Api.Services;
using Wcs.Api.RobotFlow;
using CranePosition = Wcs.Okamura.Models.CranePosition;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class AfterDropHandler(ILogger<AfterDropHandler> logger, IRcsClient rcsClient, IEventPublisher publisher, ITaskStateStore store, IOpcUaClient opcUaClient, IStationHandshakeService stationHandshakeService, ICraneService craneService, ICraneTaskNoGenerator craneTaskNoGenerator, IWmsService wmsService, TaskScopedLockService taskLocks, CraneTaskDispatchService craneTaskDispatchService)
{
  private readonly ILogger<AfterDropHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly IOpcUaClient _opcClient = opcUaClient;
  private readonly IStationHandshakeService _stationHandshakeService = stationHandshakeService;
  private readonly ICraneService _craneService = craneService;
  private readonly ICraneTaskNoGenerator _craneTaskNoGenerator = craneTaskNoGenerator;
  private readonly IWmsService _wmsService = wmsService;
  private readonly TaskScopedLockService _taskLocks = taskLocks;
  private readonly CraneTaskDispatchService _craneTaskDispatchService = craneTaskDispatchService;

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
      var station = flowTask.ToStation;
      var fromStation = flowTask.FromStation;
      var resumeImmediately = true;

      if (station.HasWarehouseDoor) {
        await using var taskLock = await _taskLocks.AcquireAsync($"crane-dispatch:{flowTask.Id}", ct);

        var latestTask = await _store.LoadAsync(flowTask.Id);
        if (latestTask is null)
        {
          _logger.LogWarning("[AfterDrop] TaskId={TaskId} không tồn tại khi chuẩn bị dispatch crane inbound", flowTask.Id);
          return;
        }

        flowTask = latestTask;
        station = flowTask.ToStation;

        if (flowTask.CraneTaskNo is not null)
        {
          _logger.LogInformation(
            "[AfterDrop] TaskId={TaskId} đã có CraneTaskNo={CraneTaskNo}, bỏ qua dispatch inbound lặp",
            flowTask.Id,
            flowTask.CraneTaskNo);
          return;
        }

        await _stationHandshakeService.Inbound_AfterDropAsync(ct, waitCraneReady: false);

        await _stationHandshakeService.WaitInboundQrReadyAsync(TimeSpan.FromSeconds(120), ct);
        var qrCode = await _stationHandshakeService.Inbound_ReadQrCodeAsync(ct);
        if (qrCode is null)
        {
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "CRANE0003", "QR code is not ready"), ct);
        }

        if (!string.IsNullOrEmpty(flowTask.CassetteCode)
            && !string.Equals(qrCode, flowTask.CassetteCode, StringComparison.Ordinal))
        {
          await _publisher.PublishAsync(
            new BizError(flowTask.Id.ToString(), "CRANE0002", $"QR code is not correct, read: {qrCode}"),
            ct);
        }

        Wcs.Common.ValueObjects.CranePosition cranePosition;
        try {
          var cranePositions = await _wmsService.FindEmptyInboundPositionsAsync(ct);
          flowTask.CranePositions = cranePositions.ToList();
          cranePosition = cranePositions[0];
        } catch (WmsApiException ex) {
          await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, "FindEmptyInboundPositions", ex, ct);
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "WMS_001", "WMS không trả về vị trí lưu kho trống"), ct);
          return;
        } catch (Exception ex) {
          await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, "FindEmptyInboundPositions", ex.Message, ct);
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "WMS_001", "Lỗi gọi WMS lấy vị trí lưu kho"), ct);
          return;
        }

        var positionCode = $"F{cranePosition.Floor}-R{cranePosition.Row}-P{cranePosition.Position}";

        try {
          await _wmsService.ConfirmPositionInboundAsync(
            position: positionCode,
            stageCode: flowTask.StorageStageCode ?? string.Empty,
            cassetteId: qrCode ?? flowTask.CassetteCode ?? string.Empty,
            size: flowTask.TaskSize.ToString(),
            quantity: flowTask.Quantity ?? 1,
            product: flowTask.Product ?? string.Empty,
            ct: ct);
        } catch (WmsApiException ex) {
          await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, $"ConfirmPositionInbound (position={positionCode})", ex, ct);
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "WMS_002", "WMS từ chối xác nhận vị trí nhập kho"), ct);
          return;
        } catch (Exception ex) {
          await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, $"ConfirmPositionInbound (position={positionCode})", ex.Message, ct);
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "WMS_002", "Lỗi gọi WMS xác nhận vị trí nhập kho"), ct);
          return;
        }

        await _stationHandshakeService.WaitInboundCraneReadyAsync(TimeSpan.FromSeconds(120), ct);

        var craneTaskNo = await _craneTaskNoGenerator.NextAsync(ct);
        var idempotencyKey = $"flow:{flowTask.Id}:crane:inbound:after-drop";
        var reservation = await _craneTaskDispatchService.ReserveFlowTaskDispatchAsync(
          flowTask.Id,
          craneTaskNo,
          Wcs.Okamura.Enums.CraneTaskType.Inbound,
          idempotencyKey,
          ct);

        craneTaskNo = reservation.Dispatch.TaskNo;
        flowTask.CraneTaskNo = craneTaskNo;
        flowTask.UpdateStep(FlowStep.AfterDrop);
        await _store.SaveAsync(flowTask);

        if (!reservation.Created)
        {
          _logger.LogInformation(
            "[AfterDrop] TaskId={TaskId} dùng lại crane dispatch đã có. TaskNo={TaskNo}, Status={Status}",
            flowTask.Id,
            reservation.Dispatch.TaskNo,
            reservation.Dispatch.Status);
          return;
        }

        var craneResult = await _craneService.SendInboundAsync(new InboundTask(
            reservation.Dispatch.TaskNo,
            CraneStationPositions.Pick,
            new CranePosition(cranePosition.Row, cranePosition.Floor, cranePosition.Position)), ct);
        if (!craneResult.Success) {
          await _craneTaskDispatchService.MarkFailedAsync(
            reservation.Dispatch.TaskNo,
            craneResult.ErrorMessage ?? "Crane inbound dispatch failed",
            craneResult.ErrorCode,
            ct);
          await FlowTaskApiErrorLogger.LogCraneAsync(_store, flowTask, "SendInbound", craneResult, ct);
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), $"CRANE0001", $"Crane error {craneResult.ErrorCode}"), ct);
          await _publisher.PublishAsync(new FlowErrorOccurred(flowTask.Id.ToString(), FlowStep.AfterDrop, $"CRANE0001"), ct);
          return;
        }

        await _craneTaskDispatchService.MarkAcceptedAsync(reservation.Dispatch.TaskNo, ct);
        return;
      } else {
        // Thực hiện các actions cần thiết
        if (!string.IsNullOrEmpty(station.Tag_Control))
        {
          await _opcClient.WriteValueAsync(station.Tag_Control, (int)StationControl.Idle);
        }
      }

      flowTask.UpdateStep(FlowStep.AfterDrop);
      await _store.SaveAsync(flowTask);

      if (resumeImmediately)
      {
        await _publisher.PublishAsync(new FlowResumed(flowTask.Id.ToString()), ct);
      }
  }
}
