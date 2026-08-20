using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class StageResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("area")]
    public string Area { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public float? Distance { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("return_empty")]
    public bool ReturnEmpty { get; set; }

    public static StageResponse FromStage(Stage stage)
    {
        return new StageResponse
        {
            Id = stage.Id,
            Name = stage.Name,
            Code = stage.Code,
            Area = stage.Area.ToString(),
            Distance = stage.Distance,
            IsActive = stage.IsActive,
            CreatedAt = stage.CreatedAt,
            UpdatedAt = stage.UpdatedAt,
            ReturnEmpty = stage.ReturnEmpty
        };
    }
}