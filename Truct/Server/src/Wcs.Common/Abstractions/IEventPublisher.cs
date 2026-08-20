namespace Wcs.Common.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : class;
    // void Subscribe<T>(Func<T, Task> handler) where T : class;
    // void Unsubscribe<T>(Func<T, Task> handler) where T : class;
}
