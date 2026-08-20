using Wcs.Common.Entities;

namespace Wcs.Common.Abstractions.Repositories;

public interface IFlowTaskRepository
{
    Task<FlowTask?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<FlowTask?> GetByRcsTaskIdAsync(string rcsTaskId, CancellationToken cancellationToken = default);
    Task<FlowTask?> GetActiveByCraneTaskNoAsync(int craneTaskNo, CancellationToken cancellationToken = default);
    /// <summary>
    /// Trả về FlowTask duy nhất đang Active ở AfterDrop và đã gán CraneTaskNo; null nếu không đúng 1 kết quả.
    /// </summary>
    Task<FlowTask?> GetSingleActiveAfterDropWithCraneAsync(CancellationToken cancellationToken = default);
    Task<FlowTask> CreateAsync(FlowTask flowTask, CancellationToken cancellationToken = default);
    Task UpdateAsync(FlowTask flowTask, CancellationToken cancellationToken = default);
    Task LogAsync(FlowTask flowTask, string logMessage, string? eventName = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetTasksInProgressOnStationAsync(string stationCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetActiveTasksByRouteAsync(string fromStationCode, string toStationCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetTasksInProgressOnRobotAsync(string robotCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetActiveTasksAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetCancelledTasksAsync(int limit = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetPendingStartTasksAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FlowTaskHistoryEntry>> GetHistoryAsync(
        string? taskId = null,
        bool errorsOnly = true,
        string? eventName = null,
        int limit = 100,
        CancellationToken cancellationToken = default);
}

