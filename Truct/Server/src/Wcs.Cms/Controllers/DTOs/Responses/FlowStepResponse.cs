using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class FlowStepResponse
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

    [JsonPropertyName("steps")]
    public List<FlowStepItem> Steps { get; set; } = [];

    public static FlowStepResponse FromFlow(Flow flow)
    {
        return new FlowStepResponse
        {
            Id = flow.Id,
            Name = flow.Name,
            StageArea = flow.Area.ToString(),
            Priority = flow.Priority.ToString(),
            ReturnEmpty = flow.ReturnEmpty,
            Status = (int)flow.Status,
            Steps = flow.Steps?.OrderBy(s => s.StepNo).Select(FlowStepItem.FromStep).ToList() ?? []
        };
    }
}