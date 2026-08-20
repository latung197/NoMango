namespace Wcs.Okamura.Models;

using Wcs.Okamura.Enums;

public abstract record CraneTask(int TaskNo);

public record CranePosition(
    int Row,
    int Floor,
    int Position
);

public record InboundTask(
    int TaskNo,
    CranePosition PickPosition,
    CranePosition PutPosition
) : CraneTask(TaskNo);

public record OutboundTask(
    int TaskNo,
    CranePosition PickPosition,
    CranePosition PutPosition
) : CraneTask(TaskNo);

public record InternalMoveTask(
    int TaskNo,
    CranePosition PickPosition,
    CranePosition PutPosition
) : CraneTask(TaskNo);

public record StationMoveTask(
    int TaskNo
) : CraneTask(TaskNo);

public record CraneTaskResult(
    bool Success,
    TaskCompleteStatus Status,
    CraneSpecialErrorType SpecialError,
    int ErrorCode,
    string? ErrorMessage
);

/// <summary>Trạng thái sẵn sàng nhận lệnh mới của crane (D3050, D3053, D3051, D3057).</summary>
public record CraneReadyStatus(
    bool IsReady,
    int TaskExecuteFeedback,
    int TaskFinishState,
    int ErrorCode,
    int DispatchReady,
    string? Reason
);

public record CraneFeedback(
    int TaskNo,
    CraneTaskType TaskType,
    TaskExecuteFeedback ExecuteFeedback,
    TaskCompleteStatus CompleteStatus,
    CraneSpecialErrorType SpecialError,
    CraneStatus Status,
    CranePosition Position,
    int ErrorCode,
    string? ErrorNote
);

public record HeartbeatStatus(
    bool Ok,
    DateTime LastUpdateTime,
    int LastValue
);
