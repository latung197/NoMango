using Wcs.Rcs.Contracts;

using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Api.RobotFlow;
using Wcs.Api.RobotFlow.Flow;
using Wcs.Rcs.DTOs;
using Wcs.OpcUa.Contracts;
using Wcs.Okamura.Services;
using Wcs.Rcs;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class EnterPickupPointHandler(ILogger<EnterPickupPointHandler> logger, IRcsClient rcsClient, IEventPublisher publisher, ITaskStateStore store, IOpcUaClient opcUaClient, IStationHandshakeService stationHandshakeService)
{
  private readonly ILogger<EnterPickupPointHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly IOpcUaClient _opcClient = opcUaClient;
  private readonly IStationHandshakeService _stationHandshakeService = stationHandshakeService;
  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
      var station = flowTask.FromStation;

      if (station.HasWarehouseDoor) {
        await _stationHandshakeService.Outbound_EnterPickupPointAsync(ct);
      } else {
        // Thực hiện các actions cần thiết
        if (!string.IsNullOrEmpty(station.Tag_Control))
        {
          await _opcClient.WriteValueAsync(station.Tag_Control, (int)StationControl.IntrusionInProgress);
        }
      }

      await _rcsClient.ContinueTaskOrThrowAsync(_store, flowTask, new ContinueTaskRequest(
        flowTask.RcsTaskId ?? throw new InvalidDataException("RCS task ID is null"),
        RcsPositions.Berth(flowTask.FromStation.MainPoint)
      ), ct);

      flowTask.UpdateStep(FlowStep.EnterPickupPoint);
      await _store.SaveAsync(flowTask);
  }
}