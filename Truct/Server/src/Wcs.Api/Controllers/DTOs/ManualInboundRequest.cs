using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Bật/tắt chế độ nhập kho thủ công (rèm an toàn mute).
/// </summary>
public class ManualInboundRequest
{
    /// <summary>1 = bật chế độ thủ công (mở rèm), 0 = tắt chế độ (đóng rèm).</summary>
    [JsonPropertyName("open")]
    [Range(0, 1, ErrorMessage = "open phải là 0 hoặc 1")]
    public int Open { get; set; }
}
