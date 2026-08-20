using Wcs.Common.Abstractions;

namespace Wcs.Api.EventTransformers;

/// <summary>
/// Registry quản lý tất cả các event transformers
/// Cho phép đăng ký và tìm kiếm transformers theo event type
/// </summary>
public class EventTransformerRegistry(ILogger<EventTransformerRegistry> logger)
{
    private readonly ILogger<EventTransformerRegistry> _logger = logger;
    private readonly Dictionary<Type, List<object>> _transformers = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// Đăng ký một transformer cho event type
    /// </summary>
    public void Register<TInput>(IEventTransformer<TInput> transformer) where TInput : class
    {
        lock (_lockObject)
        {
            var eventType = typeof(TInput);
            
            if (!_transformers.ContainsKey(eventType))
            {
                _transformers[eventType] = new List<object>();
            }

            _transformers[eventType].Add(transformer);
            
            _logger.LogInformation("Đã đăng ký transformer {TransformerType} cho event type {EventType}",
                transformer.GetType().Name, eventType.Name);
        }
    }

    /// <summary>
    /// Lấy tất cả transformers cho một event type
    /// </summary>
    public IEnumerable<IEventTransformer<TInput>> GetTransformers<TInput>() where TInput : class
    {
        lock (_lockObject)
        {
            var eventType = typeof(TInput);
            
            if (!_transformers.TryGetValue(eventType, out var transformerList))
            {
                return Enumerable.Empty<IEventTransformer<TInput>>();
            }

            return transformerList.Cast<IEventTransformer<TInput>>();
        }
    }

    /// <summary>
    /// Transform một event qua tất cả transformers đã đăng ký
    /// Trả về tất cả domain events được tạo ra
    /// </summary>
    public async Task<IEnumerable<object>> TransformAsync<TInput>(TInput input, CancellationToken cancellationToken = default) where TInput : class
    {
        _logger.LogDebug("Transforming event {EventType}", typeof(TInput).Name);
        var transformers = GetTransformers<TInput>();
        var allEvents = new List<object>();

        foreach (var transformer in transformers)
        {
            try
            {
                if (!transformer.CanTransform(input))
                {
                    _logger.LogInformation("Transformer {TransformerType} không thể transform event {EventType}",
                        transformer.GetType().Name, typeof(TInput).Name);
                    continue;
                }

                var events = await transformer.TransformAsync(input, cancellationToken);
                allEvents.AddRange(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi transform event qua transformer {TransformerType}",
                    transformer.GetType().Name);
            }
        }

        return allEvents;
    }

    /// <summary>
    /// Đếm số lượng transformers đã đăng ký
    /// </summary>
    public int GetTransformerCount()
    {
        lock (_lockObject)
        {
            return _transformers.Values.Sum(list => list.Count);
        }
    }

    /// <summary>
    /// Lấy danh sách tất cả event types đã đăng ký transformers
    /// </summary>
    public IEnumerable<Type> GetRegisteredEventTypes()
    {
        lock (_lockObject)
        {
            return _transformers.Keys.ToList();
        }
    }
}

