using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class StageByAreaListRequest
{
    [JsonPropertyName("area")]
    public string? Area { get; set; }
}