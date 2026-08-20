using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class CassetteResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Tên cassette
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã cassette
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Mã sản phẩm
    /// </summary>
    [JsonPropertyName("product")]
    public string? Product { get; set; } = string.Empty;

    /// <summary>
    /// Size cassette (enum int: 1=S, 2=M, 3=L, 4=XL, 5=LL)
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }

    /// <summary>
    /// Sức chứa hàng
    /// </summary>
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>
    /// Trạng thái hoạt động (true = hiển thị, false = không hiển thị/đã xóa)
    /// </summary>
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Thời điểm tạo
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Thời điểm cập nhật lần cuối
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public static CassetteResponse FromCassette(Cassette cassette)
    {
        return new CassetteResponse
        {
            Id = cassette.Id,
            Name = cassette.Name,
            Code = cassette.Code,
            Product = cassette.Product,
            Size = (int)cassette.Size,
            Capacity = cassette.Capacity,
            Quantity = cassette.Quantity,
            IsActive = cassette.IsActive,
            CreatedAt = cassette.CreatedAt,
            UpdatedAt = cassette.UpdatedAt
        };
    }
}
