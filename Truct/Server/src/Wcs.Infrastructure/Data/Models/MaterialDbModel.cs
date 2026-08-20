using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wcs.Infrastructure.Data.Models;

[Table("Materials")]
public class MaterialDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Tên vật liệu
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Mã vật liệu
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Đơn vị
    /// </summary>
    [Required]
    [MaxLength(15)]
    public string Unit { get; set; } = string.Empty;

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
    public virtual ICollection<MaterialRequestDbModel> MaterialRequests { get; set; } = [];
}
