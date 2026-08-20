using Wcs.Rcs.Contracts;

using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Api.RobotFlow.Flow;
using Wcs.Rcs.DTOs;
using Wcs.OpcUa.Contracts;
using Wcs.Okamura.Services;
using Wcs.Api.RobotFlow;
using Wcs.Api.Services;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class AfterPickupHandler(ILogger<AfterPickupHandler> logger, IRcsClient rcsClient, IEventPublisher publisher, ITaskStateStore store, IOpcUaClient opcUaClient, IStationHandshakeService stationHandshakeService, IWmsService wmsService)
{
  private readonly ILogger<AfterPickupHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly IOpcUaClient _opcClient = opcUaClient;
  private readonly IStationHandshakeService _stationHandshakeService = stationHandshakeService;
  private readonly IWmsService _wmsService = wmsService;

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
      var station = flowTask.FromStation;

      if (station.HasWarehouseDoor) {
        await _stationHandshakeService.Outbound_AfterPickupAsync(ct);

        var cranePosition = flowTask.CranePositions?.FirstOrDefault()
          ?? throw new InvalidOperationException("Crane position is required for warehouse outbound");
        var positionCode = $"F{cranePosition.Floor}-R{cranePosition.Row}-P{cranePosition.Position}";
        try {
          await _wmsService.CompleteTransferOutboundAsync(positionCode, ct);
        } catch (WmsApiException ex) {
          await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, $"CompleteTransferOutbound (position={positionCode})", ex, ct);
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "WMS_003", "WMS từ chối hoàn tất xuất kho"), ct);
          return;
        } catch (Exception ex) {
          await FlowTaskApiErrorLogger.LogWmsAsync(_store, flowTask, $"CompleteTransferOutbound (position={positionCode})", ex.Message, ct);
          await _publisher.PublishAsync(new BizError(flowTask.Id.ToString(), "WMS_003", "Lỗi gọi WMS hoàn tất xuất kho"), ct);
          return;
        }
      } else {
        // Thực hiện các actions cần thiết
        if (!string.IsNullOrEmpty(station.Tag_Control))
        {
          await _opcClient.WriteValueAsync(station.Tag_Control, (int)StationControl.Idle);
        }
      }

      flowTask.UpdateStep(FlowStep.AfterPickup);
      await _store.SaveAsync(flowTask);

      await _publisher.PublishAsync(new FlowResumed(flowTask.Id.ToString()), ct);
  }
}
