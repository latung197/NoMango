using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class StepResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("flow_id")]
    public Guid FlowId { get; set; }
    
    [JsonPropertyName("step")]
    public int StepNo { get; set; }

    [JsonPropertyName("stage")]
    public string Stage { get; set; } = string.Empty;

    public static StepResponse FromStep(Step step)
    {
        return new StepResponse
        {
            Id = step.Id,
            FlowId = step.FlowId,
            StepNo = step.StepNo,
            Stage = step.Stage
        };
    }
}