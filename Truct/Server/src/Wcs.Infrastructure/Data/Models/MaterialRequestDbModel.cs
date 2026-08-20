using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wcs.Infrastructure.Data.Models;

[Table("MaterialRequests")]
public class MaterialRequestDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// ID của request
    /// </summary>
    [Required]
    [MaxLength(50)]
    public Guid RequestId { get; set; }

    /// <summary>
    /// ID của vật liệu
    /// </summary>
    [Required]
    [MaxLength(50)]
    public Guid MaterialId { get; set; }

    /// <summary>
    /// Số lượng yêu cầu decimal double
    /// </summary>
    [Required]
    public float Quantity { get; set; }

    /// <summary>
    /// Số lượng đã lấy
    /// </summary>
    [Required]
    public float Actual { get; set; } = 0;

    /// <summary>
    /// Ghi chú
    /// </summary>
    [MaxLength(255)]
    public string Note { get; set; } = string.Empty;

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

    // Navigation properties
    [ForeignKey("RequestId")]
    public virtual RequestDbModel Request { get; set; } = null!;

    [ForeignKey("MaterialId")]
    public virtual MaterialDbModel Material { get; set; } = null!;
}
