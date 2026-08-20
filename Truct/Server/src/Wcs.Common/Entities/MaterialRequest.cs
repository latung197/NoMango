using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class MaterialRequest
{
    /// <summary>
    /// ID của material request
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID của request
    /// </summary>
    public Guid RequestId { get; set; }

    /// <summary>
    /// ID của vật liệu
    /// </summary>
    public Guid MaterialId { get; set; }

    /// <summary>
    /// Tên vật liệu
    /// </summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>
    /// Mã vật liệu
    /// </summary>
    public string MaterialCode { get; set; } = string.Empty;

    /// <summary>
    /// Đơn vị của vật liệu
    /// </summary>
    public string MaterialUnit { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái
    /// </summary>
    //public RequestStep CurrentStep { get; set; } = RequestStep.Initial;

    /// <summary>
    /// Số lượng yêu cầu decimal double
    /// </summary>
    public float Quantity { get; set; }

    /// <summary>
    /// Số lượng đã lấy
    /// </summary>
    public float Actual { get; set; } = 0;

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Trạng thái hoạt động (1 = hiển thị, 0 = không hiển thị/đã xóa)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Thời điểm tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm cập nhật lần cuối
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Default constructor
    public MaterialRequest() { }

    // Main constructor
    public MaterialRequest(
        Guid request_id,
        Guid material_id,
        float quantity,
        string note,
        bool isActive = true)
    {
        Id = Guid.NewGuid();
        RequestId = request_id;
        MaterialId = material_id;
        Quantity = quantity;
        Note = note;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
