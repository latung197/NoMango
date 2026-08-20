using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("Requests")]
public class RequestDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    //[Required]
    [MaxLength(10)]
    public string? FlowTaskId { get; set; }

    /// <summary>
    /// Mã request
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Tên khu vực request
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string StageCode { get; set; } = string.Empty;

    /// <summary>
    /// Mã vận chuyển
    /// </summary>
    [MaxLength(50)]
    public string? DeliveryCode { get; set; } = string.Empty;

    /// <summary>
    /// Thứ tự vận chuyển
    /// </summary>
    public int? DeliveryOrder { get; set; }

    /// <summary>
    /// Trạng thái
    /// </summary>
    public RequestStep CurrentStep { get; set; } = RequestStep.Initial;

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

    //[ForeignKey("FlowTaskId")]
    //public virtual FlowTaskDbModel? FlowTask { get; set; } = null!;
}
