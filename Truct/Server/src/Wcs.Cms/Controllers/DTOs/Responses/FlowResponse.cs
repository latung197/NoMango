using System.Text.Json.Serialization;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class FlowStepItem
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("flow_id")]
    public Guid FlowId { get; set; }

    [JsonPropertyName("step")]
    public int Step { get; set; }

    [JsonPropertyName("stage")]
    public string Stage { get; set; } = string.Empty;

    /// <summary>Size bước flow (enum int: 1=S, 2=M, 3=L, 4=XL, 5=LL). Null = tất cả size.</summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    public static FlowStepItem FromStep(Step step)
    {
        return new FlowStepItem
        {
            Id = step.Id,
            FlowId = step.FlowId,
            Step = step.StepNo,
            Stage = step.Stage,
            Size = step.Size.HasValue ? (int)step.Size.Value : null
        };
    }
}

public class FlowResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("area")]
    public string StageArea { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("return_empty")]
    public bool ReturnEmpty { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("steps")]
    public List<FlowStepItem> Steps { get; set; } = [];

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public static FlowResponse FromFlow(Flow flow)
    {
        // Map priority enum to Vietnamese
        var priorityText = flow.Priority switch
        {
            FlowPriority.High => "Cao",
            FlowPriority.Medium => "Trung",
            FlowPriority.Low => "Thấp",
            _ => "Trung"
        };

        return new FlowResponse
        {
            Id = flow.Id,
            Name = flow.Name,
            StageArea = flow.Area.ToString(),
            Priority = priorityText,
            ReturnEmpty = flow.ReturnEmpty,
            Status = (int)flow.Status,
            IsActive = flow.IsActive,
            Steps = flow.Steps?.OrderBy(s => s.StepNo).Select(FlowStepItem.FromStep).ToList() ?? [],
            CreatedAt = flow.CreatedAt,
            UpdatedAt = flow.UpdatedAt
        };
    }
}
