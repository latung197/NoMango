namespace Wcs.Common.Abstractions;

/// <summary>
/// Interface để transform low-level events thành domain events
/// </summary>
/// <typeparam name="TInput">Low-level event type (OpcTagChangedEvent, WebhookEvent, etc.)</typeparam>
public interface IEventTransformer<in TInput> where TInput : class
{
    /// <summary>
    /// Transform một event thành danh sách các domain events
    /// Trả về empty list nếu không transform được hoặc không cần transform
    /// </summary>
    Task<IEnumerable<object>> TransformAsync(TInput input, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Kiểm tra xem transformer này có thể xử lý event này không
    /// </summary>
    bool CanTransform(TInput input);
}

