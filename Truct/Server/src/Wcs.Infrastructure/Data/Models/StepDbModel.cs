using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("FlowSteps")]
public class StepDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public Guid FlowId { get; set; }

    [Required]
    public int StepNo { get; set; }

    [Required]
    [MaxLength(50)]
    public string Stage { get; set; } = string.Empty;

    public Size? Size { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // other properties...
    // Navigation properties
    [ForeignKey("FlowId")]
    public virtual FlowDbModel Flow { get; set; } = null!;
}
