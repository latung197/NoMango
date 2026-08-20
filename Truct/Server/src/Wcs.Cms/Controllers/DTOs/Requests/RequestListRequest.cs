using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class RequestListRequest 
{
    [JsonPropertyName("stage")]
    public string? Stage { get; set; }
}

