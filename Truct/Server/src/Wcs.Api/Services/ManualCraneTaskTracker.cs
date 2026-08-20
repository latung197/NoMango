using System.Collections.Concurrent;

namespace Wcs.Api.Services;

public record ManualCraneTaskState(
    int TaskNo,
    string PositionCode,
    bool CallWmsComplete,
    string StageCode,
    string CassetteId,
    string Size,
    int Quantity);

public interface IManualCraneTaskTracker
{
    void TrackInbound(
        int taskNo,
        string positionCode,
        bool callWmsComplete,
        string stageCode,
        string cassetteId,
        string size,
        int quantity);
    bool TryGet(int taskNo, out ManualCraneTaskState state);
    void Remove(int taskNo);
}

public sealed class ManualCraneTaskTracker : IManualCraneTaskTracker
{
    private readonly ConcurrentDictionary<int, ManualCraneTaskState> _tasks = new();

    public void TrackInbound(
        int taskNo,
        string positionCode,
        bool callWmsComplete,
        string stageCode,
        string cassetteId,
        string size,
        int quantity)
    {
        _tasks[taskNo] = new ManualCraneTaskState(
            taskNo,
            positionCode,
            callWmsComplete,
            stageCode,
            cassetteId,
            size,
            quantity);
    }

    public bool TryGet(int taskNo, out ManualCraneTaskState state)
        => _tasks.TryGetValue(taskNo, out state!);

    public void Remove(int taskNo)
        => _tasks.TryRemove(taskNo, out _);
}
