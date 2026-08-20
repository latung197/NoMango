using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("Cassettes")]
public class CassetteDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Tên cassette
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã cassette
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Mã sản phẩm
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string? Product { get; set; } = string.Empty;

    /// <summary>
    /// Kích thước cassette (enum int: S=1, M=2, L=3, XL=4, LL=5)
    /// </summary>
    [Required]
    public Size Size { get; set; }

    /// <summary>
    /// Sức chứa hàng
    /// </summary>
    [Required]
    public int Capacity { get; set; } = 0;

    /// <summary>
    /// Số lượng
    /// </summary>
    [Required]
    public int? Quantity { get; set; } = 0;

    /// <summary>
    /// Trạng thái hoạt động (1 = hiển thị, 0 = không hiển thị/đã xóa)
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Thời điểm tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời điểm cập nhật lần cuối
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
