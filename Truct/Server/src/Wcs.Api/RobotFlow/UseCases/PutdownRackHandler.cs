using Wcs.Rcs.Contracts;

using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Rcs.DTOs;
using Wcs.Common.ValueObjects;
using Wcs.Api.RobotFlow;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class PutdownRackHandler(ILogger<PutdownRackHandler> logger, IRcsClient rcsClient, IEventPublisher publisher, ITaskStateStore store)
{
  private readonly ILogger<PutdownRackHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
    await _rcsClient.ContinueTaskOrThrowAsync(_store, flowTask, new ContinueTaskRequest(flowTask.RcsTaskId ?? throw new InvalidDataException("RCS task ID is null")), ct);

    flowTask.UpdateStep(FlowStep.PutdownRack);
    await _store.SaveAsync(flowTask);
  }
}