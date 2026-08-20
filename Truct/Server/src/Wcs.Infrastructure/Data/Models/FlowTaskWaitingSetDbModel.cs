using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wcs.Infrastructure.Data.Models;

[Table("FlowTaskWaitingSets")]
public class FlowTaskWaitingSetDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string FlowTaskId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NeedType { get; set; } = string.Empty;

    public bool IsMet { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("FlowTaskId")]
    public virtual FlowTaskDbModel FlowTask { get; set; } = null!;
}
