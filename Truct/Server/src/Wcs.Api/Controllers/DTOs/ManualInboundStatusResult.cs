using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Trạng thái rèm an toàn nhập kho (D2024).
/// </summary>
public class ManualInboundStatusResult
{
    [JsonPropertyName("tagId")]
    public string TagId { get; init; } = "D2024";

    [JsonPropertyName("nodeId")]
    public string NodeId { get; init; } = string.Empty;

    /// <summary>0 = rèm bật, 1 = rèm tắt (chế độ thủ công).</summary>
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
