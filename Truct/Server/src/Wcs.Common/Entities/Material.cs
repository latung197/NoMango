namespace Wcs.Common.Entities;

public sealed class Material
{
    public Guid Id { get; set; }

    /// <summary>
    /// Tên vật liệu
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã vật liệu
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Đơn vị
    /// </summary>
    public string Unit { get; set; } = string.Empty;

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
    public Material() { }

    // Main constructor
    public Material(
        string name,
        string code,
        string unit,
        string note,
        bool isActive = true)
    {
        Id = Guid.NewGuid();
        Name = name;
        Code = code;
        Unit = unit;
        Note = note;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
