using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Trạng thái rèm an toàn xuất kho (D2026).
/// </summary>
public class ManualOutboundStatusResult
{
    [JsonPropertyName("tagId")]
    public string TagId { get; init; } = "D2026";

    [JsonPropertyName("nodeId")]
    public string NodeId { get; init; } = string.Empty;

    [JsonPropertyName("safetySensorStatus")]
    public int? SafetySensorStatus { get; init; }

    [JsonPropertyName("isManualMode")]
    public bool IsManualMode { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonPropertyName("isGood")]
    public bool IsGood { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}
