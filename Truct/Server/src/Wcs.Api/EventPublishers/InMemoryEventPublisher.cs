using Wcs.Common.Abstractions;

namespace Wcs.Api.EventPublishers;

public class InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger) : IEventPublisher
{
    private readonly ILogger<InMemoryEventPublisher> _logger = logger;
    private readonly List<Func<object, Task>> _subscribers = new();
    private readonly object _lockObject = new();

    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        if (@event == null) return Task.CompletedTask;

        _logger.LogDebug("Publishing event: {EventType}", @event.GetType().Name);
        
        lock (_lockObject)
        {
            var tasks = _subscribers.Select(subscriber => subscriber(@event));
            return Task.WhenAll(tasks);
        }
    }

    public Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class
    {
        if (events == null || !events.Any()) return Task.CompletedTask;

        _logger.LogDebug("Publishing {Count} events", events.Count());
        
        var tasks = events.Select(@event => PublishAsync(@event, cancellationToken));
        return Task.WhenAll(tasks);
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        lock (_lockObject)
        {
            _subscribers.Add(async (obj) =>
            {
                if (obj is T typedEvent)
                {
                    try
                    {
                        await handler(typedEvent);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi xử lý event {EventType}", typeof(T).Name);
                    }
                }
            });
        }
        
        _logger.LogInformation("Đã đăng ký subscriber cho event type {EventType}", typeof(T).Name);
    }

    public void Unsubscribe<T>(Func<T, Task> handler) where T : class
    {
        // Note: In a simple implementation, we can't easily remove specific handlers
        // For production, consider using a more sophisticated event system
        _logger.LogWarning("Unsubscribe không được hỗ trợ trong InMemoryEventPublisher");
    }
}
