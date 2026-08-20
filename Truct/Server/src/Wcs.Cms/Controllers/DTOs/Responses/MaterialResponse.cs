using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class MaterialResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Tên material
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Mã material
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Đơn vị
    /// </summary>
    [JsonPropertyName("unit")]
    public required string Unit { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    [JsonPropertyName("note")]
    public required string Note { get; set; }

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

    public static MaterialResponse FromMaterial(Material material)
    {
        return new MaterialResponse
        {
            Id = material.Id,
            Name = material.Name,
            Code = material.Code,
            Unit = material.Unit,
            Note = material.Note,
            IsActive = material.IsActive,
            CreatedAt = material.CreatedAt,
            UpdatedAt = material.UpdatedAt
        };
    }
}
