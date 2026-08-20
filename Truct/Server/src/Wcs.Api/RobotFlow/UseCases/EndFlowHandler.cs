using Wcs.Rcs.Contracts;

using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.RobotFlow.UseCases;

public sealed class EndFlowHandler(ILogger<EndFlowHandler> logger, IRcsClient rcsClient, IEventPublisher publisher, ITaskStateStore store)
{
  private readonly ILogger<EndFlowHandler> _logger = logger;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly IEventPublisher _publisher = publisher;
  private readonly ITaskStateStore _store = store;

  public async Task Handle(FlowTask flowTask, CancellationToken ct)
  {
    flowTask.Status = FlowStatus.Completed;
    flowTask.Complete();
    await _store.SaveAsync(flowTask);
  }
}