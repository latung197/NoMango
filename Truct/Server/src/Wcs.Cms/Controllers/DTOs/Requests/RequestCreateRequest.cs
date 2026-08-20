using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class RequestCreateRequest
{
    /// <summary>
    /// Mã request
    /// </summary>
    [Required(ErrorMessage = "Mã yêu cầu không được để trống")]
    [MaxLength(50, ErrorMessage = "Mã yêu cầu tối đa 50 ký tự")]
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Tên khu vực request
    /// </summary>
    [Required(ErrorMessage = "Khu vực yêu cầu không được để trống")]
    [MaxLength(50, ErrorMessage = "Khu vực yêu cầu tối đa 50 ký tự")]
    [JsonPropertyName("stageCode")]
    public required string StageCode { get; set; }

    /// <summary>
    /// Danh sách vật liệu request
    /// </summary>
    public List<MaterialRequestDto> Materials { get; set; } = [];

}
