using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Kết quả bật/tắt chế độ nhập kho thủ công.
/// </summary>
public class ManualInboundResult
{
    /// <summary>1 = chế độ thủ công đang bật, 0 = đã tắt.</summary>
    [JsonPropertyName("open")]
    public int Open { get; init; }

    /// <summary>D2024 – 0=rèm bật, 1=rèm tắt (mute/chế độ thủ công).</summary>
    [JsonPropertyName("safetySensorStatus")]
    public int SafetySensorStatus { get; init; }

    /// <summary>Thời điểm hết hạn cửa sổ thao tác (UTC), chỉ có khi open=1.</summary>
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; init; }

    /// <summary>Thời lượng hiệu lực (giây), chỉ có khi open=1.</summary>
    [JsonPropertyName("validSeconds")]
    public int? ValidSeconds { get; init; }

    /// <summary>Các bước handshake đã thực hiện.</summary>
    [JsonPropertyName("stepsCompleted")]
    public IReadOnlyList<string> StepsCompleted { get; init; } = [];
}
