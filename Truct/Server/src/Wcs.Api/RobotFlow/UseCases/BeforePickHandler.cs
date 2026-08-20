using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.OpcUa.Contracts;
using Wcs.Api.Services;
using Wcs.Okamura.Services;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class BeforePickHandler(ILogger<BeforePickHandler> logger, IOpcUaClient opcUaClient, IEventPublisher publisher, ITaskStateStore store, StationService stationService, IStationHandshakeService stationHandshakeService)
{
  private readonly ILogger<BeforePickHandler> _logger = logger;
  private readonly IOpcUaClient _opcClient = opcUaClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly StationService _stationService = stationService;
  private readonly IStationHandshakeService _stationHandshakeService = stationHandshakeService;

  /// <summary>Đổi thành <c>true</c> để bật log trace debug BeforePick.</summary>
  private static readonly bool EnableTraceLog = true;

  private void TraceLog(string message, params object?[] args)
  {
    if (!EnableTraceLog) return;
    _logger.LogInformation(message, args);
  }

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
    var station = flowTask.FromStation;
    TraceLog(
      "[BeforePick] Bắt đầu TaskId={TaskId}, Station={StationCode}, HasWarehouseDoor={HasWarehouseDoor}",
      flowTask.Id, station.Code, station.HasWarehouseDoor);

    if (station.HasWarehouseDoor) {
      TraceLog("[BeforePick] TaskId={TaskId} — chờ crane sẵn sàng (D2022=1), timeout 240s", flowTask.Id);
      await _stationHandshakeService.WaitOutboundCraneReadyAsync(TimeSpan.FromSeconds(240), ct);
      TraceLog("[BeforePick] TaskId={TaskId} — crane sẵn sàng", flowTask.Id);

      TraceLog("[BeforePick] TaskId={TaskId} — chờ có hàng tại cửa kho (D2027=1), timeout 240s", flowTask.Id);
      await _stationHandshakeService.WaitOutboundHasBoxAsync(TimeSpan.FromSeconds(240), ct);
      TraceLog("[BeforePick] TaskId={TaskId} — D2027=1, có hàng tại cửa kho", flowTask.Id);

      TraceLog("[BeforePick] TaskId={TaskId} — chờ QR sẵn sàng đọc (D2321=1), timeout 240s", flowTask.Id);
      await _stationHandshakeService.WaitOutboundQrReadyAsync(TimeSpan.FromSeconds(240), ct);
      TraceLog("[BeforePick] TaskId={TaskId} — D2321=1, QR sẵn sàng đọc", flowTask.Id);

      TraceLog("[BeforePick] TaskId={TaskId} — đọc QR code", flowTask.Id);
      var qrCode = await _stationHandshakeService.Outbound_ReadQrCodeAsync(ct);
      TraceLog("[BeforePick] TaskId={TaskId} — đọc QR xong, QrCode={QrCode}", flowTask.Id, qrCode ?? "(null)");
      if (qrCode is null) {
        await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "CRANE0003", "QR code is not ready"), ct);
      }

      TraceLog("[BeforePick] TaskId={TaskId} — Outbound_BeforePick, timeout 30s", flowTask.Id);
      await _stationHandshakeService.Outbound_BeforePickAsync(TimeSpan.FromSeconds(30), ct);
      TraceLog("[BeforePick] TaskId={TaskId} — Outbound_BeforePick xong", flowTask.Id);
    } else {
      if (!string.IsNullOrEmpty(station.Tag_Control))
      {
        TraceLog(
          "[BeforePick] TaskId={TaskId} — ghi OPC IntrusionRequest, Tag={Tag}",
          flowTask.Id, station.Tag_Control);
        await _opcClient.WriteValueAsync(station.Tag_Control, (int)StationControl.IntrusionRequest);
        TraceLog("[BeforePick] TaskId={TaskId} — ghi OPC xong", flowTask.Id);
      }
      else
      {
        TraceLog("[BeforePick] TaskId={TaskId} — không có Tag_Control, bỏ qua OPC", flowTask.Id);
      }
    }

    TraceLog("[BeforePick] TaskId={TaskId} — cập nhật step và lấy WaitingRequirements", flowTask.Id);
    flowTask.UpdateStep(FlowStep.BeforePick);
    var waitingFor = await _stationService.GetWaitingRequirements(FlowStep.BeforePick, station);
    flowTask.WaitingFor = waitingFor;
    await _store.SaveAsync(flowTask);
    TraceLog(
      "[BeforePick] TaskId={TaskId} — đã lưu state, IsAllMet={IsAllMet}, WaitingFor={@WaitingFor}",
      flowTask.Id, waitingFor.IsAllMet, waitingFor);

    // Nếu có gì cần chờ, publish StepWaiting
    if (!waitingFor.IsAllMet)
    {
      TraceLog(
        "[BeforePick] TaskId={TaskId} — đang chờ điều kiện tại station {StationCode}, chưa publish FlowResumed",
        flowTask.Id, station.Code);
    }
    else
    {
      TraceLog(
        "[BeforePick] TaskId={TaskId} — đủ điều kiện, publish FlowResumed → pick",
        flowTask.Id);
      await _publisher.PublishAsync(new FlowResumed(flowTask.Id.ToString()), ct);
      TraceLog("[BeforePick] TaskId={TaskId} — FlowResumed đã publish, kết thúc Handle", flowTask.Id);
    }
  }
}
