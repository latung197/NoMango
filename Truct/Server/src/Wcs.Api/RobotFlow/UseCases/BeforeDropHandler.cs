using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.OpcUa.Contracts;
using Wcs.Api.RobotFlow;
using Wcs.Api.Services;
using Wcs.Okamura.Services;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class BeforeDropHandler(ILogger<BeforeDropHandler> logger, IOpcUaClient opcUaClient, IEventPublisher publisher, ITaskStateStore store, StationService stationService, IStationHandshakeService stationHandshakeService)
{
  private readonly ILogger<BeforeDropHandler> _logger = logger;
  private readonly IOpcUaClient _opcClient = opcUaClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly StationService _stationService = stationService;
  private readonly IStationHandshakeService _stationHandshakeService = stationHandshakeService;

  /// <summary>Đổi thành <c>true</c> để bật log trace debug BeforeDrop.</summary>
  private static readonly bool EnableTraceLog = true;

  private void TraceLog(string message, params object?[] args)
  {
    if (!EnableTraceLog) return;
    _logger.LogInformation(message, args);
  }

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
    var station = flowTask.ToStation;
    TraceLog(
      "[BeforeDrop] Bắt đầu TaskId={TaskId}, Station={StationCode}, HasWarehouseDoor={HasWarehouseDoor}",
      flowTask.Id, station.Code, station.HasWarehouseDoor);

    if (station.HasWarehouseDoor) {
      TraceLog("[BeforeDrop] TaskId={TaskId} — Inbound_BeforeDrop, chờ D2001+D2002+D2003=1 và D2028=0, timeout 240s", flowTask.Id);
      try
      {
        await _stationHandshakeService.Inbound_BeforeDropAsync(TimeSpan.FromSeconds(240), ct);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Inbound_BeforeDropAsync failed for task {TaskId}", flowTask.Id);
        await FlowTaskApiErrorLogger.LogCraneAsync(_store, flowTask, "Inbound_BeforeDropAsync", ex);
        throw;
      }
      TraceLog("[BeforeDrop] TaskId={TaskId} — Inbound_BeforeDrop xong (CV sẵn sàng)", flowTask.Id);
    } else {
      if (!string.IsNullOrEmpty(station.Tag_Control))
      {
        TraceLog(
          "[BeforeDrop] TaskId={TaskId} — ghi OPC IntrusionRequest, Tag={Tag}",
          flowTask.Id, station.Tag_Control);
        await _opcClient.WriteValueAsync(station.Tag_Control, (int)StationControl.IntrusionRequest);
        TraceLog("[BeforeDrop] TaskId={TaskId} — ghi OPC xong", flowTask.Id);
      }
      else
      {
        TraceLog("[BeforeDrop] TaskId={TaskId} — không có Tag_Control, bỏ qua OPC", flowTask.Id);
      }
    }

    TraceLog("[BeforeDrop] TaskId={TaskId} — cập nhật step và lấy WaitingRequirements", flowTask.Id);
    flowTask.UpdateStep(FlowStep.BeforeDrop);
    var waitingFor = await _stationService.GetWaitingRequirements(FlowStep.BeforeDrop, station);
    flowTask.WaitingFor = waitingFor;
    await _store.SaveAsync(flowTask);
    TraceLog(
      "[BeforeDrop] TaskId={TaskId} — đã lưu state, IsAllMet={IsAllMet}, WaitingFor={@WaitingFor}",
      flowTask.Id, waitingFor.IsAllMet, waitingFor);

    if (!waitingFor.IsAllMet)
    {
      TraceLog(
        "[BeforeDrop] TaskId={TaskId} — đang chờ điều kiện tại station {StationCode}, chưa publish FlowResumed",
        flowTask.Id, station.Code);
    }
    else
    {
      TraceLog(
        "[BeforeDrop] TaskId={TaskId} — đủ điều kiện, publish FlowResumed → drop",
        flowTask.Id);
      await _publisher.PublishAsync(new FlowResumed(flowTask.Id.ToString()), ct);
      TraceLog("[BeforeDrop] TaskId={TaskId} — FlowResumed đã publish, kết thúc Handle", flowTask.Id);
    }
  }
}
