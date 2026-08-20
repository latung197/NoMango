using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class CassetteUpdateRequest
{
    /// <summary>
    /// Tên cassette
    /// </summary>
    [Required(ErrorMessage = "Tên cassette không được để trống")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Mã cassette
    /// </summary>
    [Required(ErrorMessage = "Mã cassette không được để trống")]
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Mã sản phẩm
    /// </summary>
    [JsonPropertyName("product")]
    public string? Product { get; set; } = string.Empty;

    /// <summary>
    /// Size cassette (enum int: 1=S, 2=M, 3=L, 4=XL, 5=XXL)
    /// </summary>
    [Required(ErrorMessage = "Kích thước cassette không được để trống")]
    [Range(1, 5, ErrorMessage = "Kích thước cassette không hợp lệ")]
    [JsonPropertyName("size")]
    public int Size { get; set; }

    /// <summary>
    /// Sức chứa hàng (phải là số nguyên dương)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Sức chứa hàng phải là số nguyên dương")]
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; } = 0;

    /// <summary>
    /// Số lượng sản phẩm
    /// </summary>
    [Range(0, 32767, ErrorMessage = "Sức chứa hàng phải là số nguyên dương < 32.767")]
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; } = 0;

    /// <summary>
    /// Trạng thái hoạt động (true = hiển thị, false = không hiển thị)
    /// </summary>
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = true;
}
