using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

public class StopAllAmrRequest
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

public class ResumeAllAmrRequest
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

public record AmrRobotControlResultDto(
    [property: JsonPropertyName("robotCode")] string RobotCode,
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message
);

public record StopAllAmrResultDto(
    [property: JsonPropertyName("stoppedCount")] int StoppedCount,
    [property: JsonPropertyName("failedCount")] int FailedCount,
    [property: JsonPropertyName("results")] IReadOnlyList<AmrRobotControlResultDto> Results
);

public record ResumeAllAmrResultDto(
    [property: JsonPropertyName("resumedCount")] int ResumedCount,
    [property: JsonPropertyName("failedCount")] int FailedCount,
    [property: JsonPropertyName("results")] IReadOnlyList<AmrRobotControlResultDto> Results
);

public record AmrControlStatusDto(
    [property: JsonPropertyName("isStopped")] bool IsStopped
);
