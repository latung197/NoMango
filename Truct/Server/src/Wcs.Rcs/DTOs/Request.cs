namespace Wcs.Rcs.DTOs;

public record CreateTaskRequest(
    string TaskTyp,     // ví dụ "MOVE_POD"
    string Priority,    // "1".."9"
    IList<PositionCodePathItem> PositionCodePath,
    string? AgvCode = null     // 18007
);

public record PositionCodePathItem(
    string Type,   // "01" berth, "02" pod...
    string PositionCode   // mã vị trí/mã pod/mã area...
);

public record ContinueTaskRequest(
    string TaskCode,
    PositionCodePathItem? NextPositionCode = null
);

public record CancelTaskRequest(
    string TaskCode,
    string? ForceCancel // "0"/"1"
);

public record QueryTaskStatusRequest(
    IList<string> TaskCodes
);

public record QueryAgvStatusRequest(
    string? RobotCode
);

public record SetTaskPriorityRequest(
    string TaskCode,
    string Priority
);

public record BindPodAndBerthRequest(
    string IndBind,
    IList<BindPodAndBerthItem> BindParam
);

public record BindPodAndBerthItem(
    string PodCode,
    string BerthCode,
    string? PodDir
);

public record BindPodAndMatRequest(
    string IndBind,
    IList<BindPodAndMatItem> BindParam
);

public record BindPodAndMatItem(
    string PodCode,
    string MatCode,
    string? PodDir
);

public record BindCtnrAndBinRequest(
    string IndBind,
    IList<BindCtnrAndBinItem> BindParam
);

public record BindCtnrAndBinItem(
    string CtnrCode,
    string BinCode,
    string? CtnrDir
);

public record LockPositionRequest(
    string IndLock,
    IList<LockPositionItem> LockParam
);

public record LockPositionItem(
    string PositionCode,
    string PositionType
);

public record SyncMapDatasRequest(
    string MapData
);

public record StopRobotRequest(
    string RobotCount,
    IList<string> Robots
);

public record ResumeRobotRequest(
    string RobotCount,
    IList<string> Robots
);

public record SetAreaStateRequest(
    string AreaCode,
    string AreaState
);