using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class ProfileResponse 
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("permissions")]
    public IEnumerable<string> Permissions { get; set; } = [];

    [JsonPropertyName("area")]
    public string Area { get; set; } = string.Empty;

    [JsonPropertyName("stageCodes")]
    public IEnumerable<string> StageCodes { get; set; } = [];
}