using DotNetCore.CAP;
using Wcs.Common.Abstractions;

namespace Wcs.Api.EventPublishers;

public sealed class CapEventPublisher(ICapPublisher cap, ILogger<CapEventPublisher> log) : IEventPublisher
{
    private readonly ICapPublisher _cap = cap;
    private readonly ILogger<CapEventPublisher> _log = log;

    // Quy ước: topic theo tên type thực tế của object (runtime type)
    private static string GetTopicFor(object @event) => @event.GetType().FullName!;

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        if (@event is null) return;
        
        // Sử dụng runtime type thay vì compile-time generic type
        // Điều này quan trọng khi T = object nhưng @event là CurtainOpened, etc.
        var topic = GetTopicFor(@event);
        _log.LogDebug("CAP publish {Topic} (actual type: {ActualType})", topic, @event.GetType().Name);
        await _cap.PublishAsync(topic, @event, cancellationToken: ct);
    }

    public Task PublishAsync<T>(IEnumerable<T> events, CancellationToken ct = default) where T : class
    {
        // tuần tự để giữ trật tự (hoặc Parallel.ForEachAsync nếu bạn muốn)
        return Task.WhenAll(events.Select(e => PublishAsync(e, ct)));
    }
}
