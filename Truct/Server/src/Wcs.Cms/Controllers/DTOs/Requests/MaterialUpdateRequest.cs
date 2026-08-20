using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class MaterialUpdateRequest
{
    /// <summary>
    /// Tên material
    /// </summary>
    [Required(ErrorMessage = "Tên vật liệu không được để trống")]
    [MaxLength(200, ErrorMessage = "Tên vật liệu tối đa 200 ký tự")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Mã material
    /// </summary>
    [Required(ErrorMessage = "Mã vật liệu không được để trống")]
    [MaxLength(50, ErrorMessage = "Mã vật liệu tối đa 50 ký tự")]
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Đơn vị material
    /// </summary>
    [Required(ErrorMessage = "Đơn vị vật liệu không được để trống")]
    [MaxLength(15, ErrorMessage = "Đơn vị vật liệu tối đa 15 ký tự")]
    [JsonPropertyName("unit")]
    public required string Unit { get; set; }

    /// <summary>
    /// Ghi chú material
    /// </summary>
    //[Required(ErrorMessage = "Ghi chú không được để trống")]
    [MaxLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự")]
    [JsonPropertyName("note")]
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái hoạt động (true = hiển thị, false = không hiển thị)
    /// </summary>
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = true;
}
