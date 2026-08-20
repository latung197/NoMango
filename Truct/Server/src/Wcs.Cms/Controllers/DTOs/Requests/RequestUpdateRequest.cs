using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class RequestUpdateRequest
{
    //[MaxLength(50, ErrorMessage = "ID vật liệu tối đa 50 ký tự")]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("flowTaskId")]
    public string? FlowTaskId { get; set; } = string.Empty;

    /// <summary>
    /// Mã material
    /// </summary>
    [Required(ErrorMessage = "Mã vật liệu không được để trống")]
    [MaxLength(50, ErrorMessage = "Mã vật liệu tối đa 50 ký tự")]
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
    /// Mã vận chuyển
    /// </summary>
    [MaxLength(50, ErrorMessage = "Khu vực yêu cầu tối đa 50 ký tự")]
    [JsonPropertyName("deliveryCode")]
    public string? DeliveryCode { get; set; }

    /// <summary>
    /// Thứ tự vận chuyển
    /// </summary>
    [JsonPropertyName("deliveryOrder")]
    public int? DeliveryOrder { get; set; }

    /// <summary>
    /// Trạng thái
    /// </summary>
    public RequestStep CurrentStep { get; set; } = RequestStep.Initial;

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
