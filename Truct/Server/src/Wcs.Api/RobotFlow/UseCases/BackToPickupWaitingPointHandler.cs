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

public sealed class BackToPickupWaitingPointHandler(ILogger<BackToPickupWaitingPointHandler> logger, IRcsClient rcsClient, IEventPublisher publisher, ITaskStateStore store, IOpcUaClient opcUaClient, IStationHandshakeService stationHandshakeService, ICraneService craneService)
{
  private readonly ILogger<BackToPickupWaitingPointHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;
  private readonly IOpcUaClient _opcClient = opcUaClient;
  private readonly IStationHandshakeService _stationHandshakeService = stationHandshakeService;
  private readonly ICraneService _craneService = craneService;
  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
      if (flowTask.FromStation.HasWarehouseDoor) {
        await _stationHandshakeService.Outbound_BackToPickupWaitingPointAsync(ct);
      }
      
      await _rcsClient.ContinueTaskOrThrowAsync(_store, flowTask, new ContinueTaskRequest(
        flowTask.RcsTaskId ?? throw new InvalidDataException("RCS task ID is null"),
        RcsPositions.Berth(flowTask.FromStation.WaitingPoint)
      ), ct);

      flowTask.UpdateStep(FlowStep.BackToPickupWaitingPoint);
      await _store.SaveAsync(flowTask);
  }
}