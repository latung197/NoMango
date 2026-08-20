using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class StepRequest
{
    [JsonPropertyName("step")]
    public int Step { get; set; }

    [JsonPropertyName("stage")]
    public string Stage { get; set; } = string.Empty;

    /// <summary>Size bước flow (enum int: 1=S, 2=M, 3=L, 4=XL, 5=LL). Null = tất cả size.</summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }
}

public class FlowCreateRequest
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
