using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class MaterialRequestDto
{
    /// <summary>
    /// ID vật liệu
    /// </summary>
    public Guid MaterialId { get; set; }

    /// <summary>
    /// Tên material
    /// </summary>
    //[Required(ErrorMessage = "Tên vật liệu không được để trống")]
    //[MaxLength(200, ErrorMessage = "Tên vật liệu tối đa 200 ký tự")]
    //[JsonPropertyName("name")]
    //public required string Name { get; set; }

    /// <summary>
    /// Số lượng request
    /// </summary>
    [Required(ErrorMessage = "Số lượng yêu cầu không được để trống")]
    [JsonPropertyName("quantity")]
    public required float Quantity { get; set; }

    //public required float Actual { get; set; } = 0;

    /// <summary>
    /// Ghi chú material
    /// </summary>
    //[Required(ErrorMessage = "Ghi chú không được để trống")]
    [MaxLength(255, ErrorMessage = "Ghi chú tối đa 255 ký tự")]
    [JsonPropertyName("note")]
    public string Note { get; set; } = string.Empty;

}
