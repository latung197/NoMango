using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Wcs.Api.Services;

public record TaskLiveEvent(
    string EventName,
    string TaskId,
    string? Detail,
    DateTime Timestamp);

public interface ITaskEventBroadcaster
{
    void Broadcast(string taskId, string eventName, string? detail = null);
    IAsyncEnumerable<TaskLiveEvent> SubscribeAsync(string taskId, CancellationToken ct);
}

/// <summary>
/// Singleton broadcaster dùng System.Threading.Channels.
/// Không lưu vào DB — events chỉ tồn tại trong memory, mất khi reload.
/// </summary>
public sealed class TaskEventBroadcaster : ITaskEventBroadcaster
{
    private readonly ConcurrentDictionary<string, List<Channel<TaskLiveEvent>>> _subs = new();

    public void Broadcast(string taskId, string eventName, string? detail = null)
    {
        if (!_subs.TryGetValue(taskId, out var channels)) return;

        var msg = new TaskLiveEvent(eventName, taskId, detail, DateTime.UtcNow);

        List<Channel<TaskLiveEvent>> snapshot;
        lock (channels) snapshot = [.. channels];

        foreach (var ch in snapshot)
            ch.Writer.TryWrite(msg);
    }

    public async IAsyncEnumerable<TaskLiveEvent> SubscribeAsync(
        string taskId,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var channel = Channel.CreateBounded<TaskLiveEvent>(new BoundedChannelOptions(200)
        {
            FullMode = BoundedChannelFullMode.DropOldest
        });

        var list = _subs.GetOrAdd(taskId, _ => []);
        lock (list) list.Add(channel);

        try
        {
            await foreach (var msg in channel.Reader.ReadAllAsync(ct))
                yield return msg;
        }
        finally
        {
            lock (list) list.Remove(channel);
            if (list.Count == 0) _subs.TryRemove(taskId, out _);
        }
    }
}
