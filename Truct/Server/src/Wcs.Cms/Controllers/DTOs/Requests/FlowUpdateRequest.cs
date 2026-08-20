using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class FlowUpdateRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [MaxLength(50, ErrorMessage = "Vui lòng nhập Tên luồng trong phạm vi tối đa 50 ký tự")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("area")]
    public required string Area { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("priority")]
    public required string Priority { get; set; }

    [JsonPropertyName("return_empty")]
    public bool ReturnEmpty { get; set; } = false;

    [JsonPropertyName("steps")]
    public List<StepRequest> Steps { get; set; } = [];
}
