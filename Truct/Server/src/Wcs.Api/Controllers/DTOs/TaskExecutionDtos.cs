using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

public class TaskExecutionControlRequest
{
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

public record TaskExecutionStatusDto(
    [property: JsonPropertyName("isPaused")] bool IsPaused
);

public record TaskExecutionResumeResultDto(
    [property: JsonPropertyName("startedCount")] int StartedCount,
    [property: JsonPropertyName("startedTaskIds")] IReadOnlyList<string> StartedTaskIds
);
