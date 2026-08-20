using DotNetCore.CAP;
using Wcs.Api.EventTransformers;
using Wcs.Common.Abstractions;
using Wcs.Common.Events;

namespace Wcs.Api.EventHandlers;

public class OpcTagChangedEventHandler(
    EventTransformerRegistry transformerRegistry,
    IEventPublisher eventPublisher,
    StationSnapshotPublisher stationSnapshotPublisher,
    ILogger<OpcTagChangedEventHandler> logger) : ICapSubscribe
{
    private readonly EventTransformerRegistry _transformerRegistry = transformerRegistry;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly StationSnapshotPublisher _stationSnapshotPublisher = stationSnapshotPublisher;
    private readonly ILogger<OpcTagChangedEventHandler> _logger = logger;

    [CapSubscribe("Wcs.Common.Events.OpcTagChangedEvent")]
    public async Task HandleOpcTagChangedEventAsync(OpcTagChangedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Nhận được OPC tag change: NodeId={NodeId}, OldValue={OldValue}, NewValue={NewValue}", 
                @event.NodeId, @event.OldValue, @event.NewValue);

        // Transform OPC tag event thành domain events
        var domainEvents = (await _transformerRegistry.TransformAsync(@event, cancellationToken)).ToList();
        await _stationSnapshotPublisher.PublishForOpcTagChangedAsync(@event, cancellationToken);

        if (!domainEvents.Any())
        {
            _logger.LogDebug("Không có domain event nào được tạo từ OPC tag {NodeId}", @event.NodeId);
            return;
        }

        // Publish tất cả domain events
        foreach (var domainEvent in domainEvents)
        {
            _logger.LogInformation("Publishing domain event {EventType} từ OPC tag {NodeId}",
                domainEvent.GetType().Name, @event.NodeId);
            
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }
    }
}
