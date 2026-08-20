using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Kết quả bật/tắt chế độ xuất kho thủ công.
/// </summary>
public class ManualOutboundResult
{
    [JsonPropertyName("open")]
    public int Open { get; init; }

    /// <summary>D2026 – 0=rèm bật, 1=rèm tắt (mute/chế độ thủ công).</summary>
    [JsonPropertyName("safetySensorStatus")]
    public int SafetySensorStatus { get; init; }

    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; init; }

    [JsonPropertyName("validSeconds")]
    public int? ValidSeconds { get; init; }

    [JsonPropertyName("stepsCompleted")]
    public IReadOnlyList<string> StepsCompleted { get; init; } = [];

    /// <summary>QR đọc được từ D2311–D2320 khi open=1, null nếu chưa đọc hoặc open=0.</summary>
    [JsonPropertyName("qrCode")]
    public string? QrCode { get; init; }
}
