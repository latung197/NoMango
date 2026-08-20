using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

public class RcsWarnCallbackRequest
{
    [JsonPropertyName("reqCode")]
    public string ReqCode { get; set; } = string.Empty;

    [JsonPropertyName("reqTime")]
    public string ReqTime { get; set; } = string.Empty;

    [JsonPropertyName("clientCode")]
    public string ClientCode { get; set; } = string.Empty;

    [JsonPropertyName("tokenCode")]
    public string TokenCode { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public List<RcsWarnCallbackData>? Data { get; set; }
}

public class RcsWarnCallbackData
{
    [JsonPropertyName("robotCode")]
    public string RobotCode { get; set; } = string.Empty;

    [JsonPropertyName("beginTime")]
    public string BeginTime { get; set; } = string.Empty;

    [JsonPropertyName("warnContent")]
    public string WarnContent { get; set; } = string.Empty;

    [JsonPropertyName("taskCode")]
    public string TaskCode { get; set; } = string.Empty;
}
