namespace Wcs.Api.RobotFlow.Flow;

using Wcs.Common.Abstractions;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

public sealed class StopFlow(
  ILogger<StopFlow> logger, ITaskStateStore store, IEventPublisher publisher
) {
  private readonly ILogger<StopFlow> _logger = logger;
  private readonly ITaskStateStore _store = store;
  private readonly IEventPublisher _publisher = publisher;

  public async Task Handle(StopEmergency e)
  {
  }
}
