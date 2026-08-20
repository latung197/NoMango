using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions;

public interface ITaskStateStore {
  Task<FlowTask?> LoadAsync(string taskId);
  Task SaveAsync(FlowTask task);
  Task LogAsync(FlowTask task, string logMessage, string? eventName = null);
  Task<FlowTask> CreateTask(string fromStation, string toStation, string? robotCode, List<CranePosition>? cranePositions = null, string? cassetteCode = null, string? storageStageCode = null, int? quantity = null, string? product = null, Size? taskSize = null);
  Task<FlowTask> CreateFallbackTask(string toStation, string robotCode, FlowStep startStep);
  Task CancelTask(string taskId, bool isUserCancel = false);
}