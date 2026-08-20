using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class ClientIpResponse
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = string.Empty;
}
