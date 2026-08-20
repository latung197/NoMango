using DotNetCore.CAP;
using Wcs.Api.EventTransformers;
using Wcs.Common.Abstractions;
using Wcs.Common.Events;

namespace Wcs.Api.EventHandlers;

public class SystemEventHandler(
    EventTransformerRegistry transformerRegistry,
    IEventPublisher eventPublisher,
    ILogger<SystemEventHandler> logger) : ICapSubscribe
{
    private readonly EventTransformerRegistry _transformerRegistry = transformerRegistry;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<SystemEventHandler> _logger = logger;

    [CapSubscribe("Wcs.Common.Events.SystemStatusNormal")]
    public void HandleSystemStatusNormalEventAsync()
    {
    }

    [CapSubscribe("Wcs.Common.Events.SystemStatusError")]
    public void HandleSystemStatusErrorEventAsync()
    {
    }
}
