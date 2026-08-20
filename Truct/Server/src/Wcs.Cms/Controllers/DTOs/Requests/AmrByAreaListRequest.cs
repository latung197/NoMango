using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class AmrByAreaListRequest
{
    [JsonPropertyName("area")]
    public string? Area { get; set; }
}