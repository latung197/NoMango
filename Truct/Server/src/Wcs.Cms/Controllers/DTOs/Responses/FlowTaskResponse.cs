using System.Text.Json.Serialization;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class FlowTaskResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("no")]
    public int No { get; set; }

    [JsonPropertyName("fromStation")]
    public Station FromStation { get; set; } = null!;

    [JsonPropertyName("toStation")]
    public Station ToStation { get; set; } = null!;

    [JsonPropertyName("robot")]
    public Robot? Robot { get; set; } = null;

    [JsonPropertyName("currentStep")]
    public FlowStep CurrentStep { get; set; } = FlowStep.Initial;

    [JsonPropertyName("waitingFor")]
    public WaitingSet? WaitingFor { get; set; }

    [JsonPropertyName("rcsTaskId")]
    public string? RcsTaskId { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("completedAt")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("currentStepStartedAt")]
    public DateTime? CurrentStepStartedAt { get; set; }

    [JsonPropertyName("logMessage")]
    public string? LogMessage { get; set; } = string.Empty;

    // Status computed property
    [JsonPropertyName("status")]
    public FlowStatus Status { get; set; } = FlowStatus.Active;

    [JsonPropertyName("isCompleted")]
    public bool IsCompleted => CurrentStep == FlowStep.Completed || CompletedAt.HasValue;

    [JsonPropertyName("isActive")]
    public bool IsActive => !IsCompleted;

    [JsonPropertyName("executionTime")]
    public TimeSpan? ExecutionTime => CompletedAt.HasValue ? CompletedAt.Value - CreatedAt : null;

    [JsonPropertyName("taskSize")]
    public Size TaskSize { get; set; } = Size.S;

    public static FlowTaskResponse FromFlowTask(FlowTask flowTask, int index = 1)
    {
        return new FlowTaskResponse
        {
            Id = flowTask.Id,
            No = index + 1,
            FromStation = flowTask.FromStation,
            ToStation = flowTask.ToStation,
            Robot = flowTask.Robot,
            CurrentStep = flowTask.CurrentStep,
            WaitingFor = flowTask.WaitingFor,
            RcsTaskId = flowTask.RcsTaskId,
            CreatedAt = flowTask.CreatedAt,
            UpdatedAt = flowTask.UpdatedAt,
            CompletedAt = flowTask.CompletedAt,
            CurrentStepStartedAt = flowTask.CurrentStepStartedAt,
            LogMessage = flowTask.LogMessage,
            Status = flowTask.Status,
            TaskSize = flowTask.TaskSize,
        };
    }
}