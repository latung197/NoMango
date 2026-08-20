using System.Threading;
using System.Threading.Tasks;
using Wcs.Rcs.DTOs;

namespace Wcs.Rcs.Contracts;

public interface IRcsClient
{
    // Core
    Task<RcsResult<string>> GenAgvSchedulingTask(CreateTaskRequest req, CancellationToken ct = default);
    Task<RcsResult<ContinueTaskResponse>> ContinueTask(ContinueTaskRequest req, CancellationToken ct = default);
    Task<RcsResult<CancelTaskResponse>> CancelTask(CancelTaskRequest req, CancellationToken ct = default);
    Task<RcsResult<IList<QueryTaskStatusResponse>>> QueryTaskStatus(QueryTaskStatusRequest req, CancellationToken ct = default);

    // Robot status (DPS service, port 8083)
    Task<RcsResult<QueryAgvStatusResponse>> QueryAgvStatus(QueryAgvStatusRequest req, CancellationToken ct = default);

    // Optional / Map & Binding
    Task<RcsResult<SetTaskPriorityResponse>> SetTaskPriority(SetTaskPriorityRequest req, CancellationToken ct = default);
    Task<RcsResult<BindPodAndBerthResponse>> BindPodAndBerth(BindPodAndBerthRequest req, CancellationToken ct = default);
    Task<RcsResult<BindPodAndMatResponse>> BindPodAndMat(BindPodAndMatRequest req, CancellationToken ct = default);
    Task<RcsResult<BindCtnrAndBinResponse>> BindCtnrAndBin(BindCtnrAndBinRequest req, CancellationToken ct = default);
    Task<RcsResult<LockPositionResponse>> LockPosition(LockPositionRequest req, CancellationToken ct = default);
    Task<RcsResult<SyncMapDatasResponse>> SyncMapDatas(SyncMapDatasRequest req, CancellationToken ct = default);

    // Safety / Traffic
    Task<RcsResult<StopRobotResponse>> StopRobot(StopRobotRequest req, CancellationToken ct = default);
    Task<RcsResult<ResumeRobotResponse>> ResumeRobot(ResumeRobotRequest req, CancellationToken ct = default);
    Task<RcsResult<SetAreaStateResponse>> SetAreaState(SetAreaStateRequest req, CancellationToken ct = default);

    // Utility
    string GenerateRequestCode();
}
