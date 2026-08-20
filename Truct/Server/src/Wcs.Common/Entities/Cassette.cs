using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Cassette
{
    public Guid Id { get; set; }

    /// <summary>
    /// Tên cassette
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã cassette
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Mã sản phẩm
    /// </summary>
    public string? Product { get; set; } = string.Empty;

    /// <summary>
    /// Size cassette (enum: S=1, M=2, L=3, XL=4, XXL=5)
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// Sức chứa hàng
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public int? Quantity { get; set; }

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

    public Cassette() { }

    public Cassette(
        string name,
        string code,
        string? product,
        Size size,
        int capacity,
        int? quantity,
        bool isActive = true)
    {
        Id = Guid.NewGuid();
        Name = name;
        Code = code;
        Product = product;
        Size = size;
        Capacity = capacity;
        Quantity = quantity;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
