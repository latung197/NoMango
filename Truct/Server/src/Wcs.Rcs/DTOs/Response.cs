using System.Text.Json.Serialization;

namespace Wcs.Rcs.DTOs;

public record RcsResult<T>(
    bool Success,
    string Code,
    string Message,
    T? Data
);

public record CreateTaskResponse(
    [property: JsonPropertyName("taskCode")] string TaskCode
);
public record ContinueTaskResponse(
    [property: JsonPropertyName("taskCode")] string TaskCode, 
    [property: JsonPropertyName("status")] string Status
);
public record CancelTaskResponse(
    [property: JsonPropertyName("taskCode")] string TaskCode, 
    [property: JsonPropertyName("status")] string Status
);
public record QueryTaskStatusResponse(
    [property: JsonPropertyName("agvCode")] string AgvCode,
    [property: JsonPropertyName("taskCode")] string TaskCode,
    [property: JsonPropertyName("taskStatus")] string TaskStatus,
    [property: JsonPropertyName("taskTyp")] string TaskTyp
);
public record QueryAgvStatusResponse(
    [property: JsonPropertyName("robots")] IList<RobotStatus> Robots
);

public record RobotStatus(
    [property: JsonPropertyName("robotCode")] string RobotCode,
    [property: JsonPropertyName("robotState")] string RobotState,
    [property: JsonPropertyName("podCode")] string? PodCode,
    [property: JsonPropertyName("berthCode")] string? BerthCode
);

public record SetTaskPriorityResponse(
    [property: JsonPropertyName("taskCode")] string TaskCode, 
    [property: JsonPropertyName("priority")] string Priority
);
public record BindPodAndBerthResponse(
    [property: JsonPropertyName("result")] string Result
);
public record BindPodAndMatResponse(
    [property: JsonPropertyName("result")] string Result
);
public record BindCtnrAndBinResponse(
    [property: JsonPropertyName("result")] string Result
);
public record LockPositionResponse(
    [property: JsonPropertyName("result")] string Result
);
public record SyncMapDatasResponse(
    [property: JsonPropertyName("result")] string Result
);
public record StopRobotResponse(
    [property: JsonPropertyName("result")] string Result
);
public record ResumeRobotResponse(
    [property: JsonPropertyName("result")] string Result
);
public record SetAreaStateResponse(
    [property: JsonPropertyName("result")] string Result
);