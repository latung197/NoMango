using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

public class RcsCallbackResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "0";

    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "successful";

    [JsonPropertyName("reqCode")]
    public string ReqCode { get; set; } = string.Empty;
}

