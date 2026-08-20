using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class AuthResponse 
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expiredTime")]
    public string ExpiredTime { get; set; } = string.Empty;
}