using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("Flows")]
public class FlowDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public StageArea Area { get; set; } = StageArea.Chip;

    [Required]
    [MaxLength(20)]
    public FlowPriority Priority { get; set; } = FlowPriority.Medium;

    [Required]
    public bool ReturnEmpty { get; set; }

    [Required]
    [MaxLength(20)]
    public FlowStatus Status { get; set; } = FlowStatus.Active;

    // Soft delete flag
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<StepDbModel> Steps { get; set; } = [];
}
