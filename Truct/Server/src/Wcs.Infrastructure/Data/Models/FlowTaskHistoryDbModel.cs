using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("FlowTaskHistory")]
public class FlowTaskHistoryDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string FlowTaskId { get; set; } = string.Empty;

    public string? Event { get; set; } = string.Empty;

    public FlowStep? Step { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? LogMessage { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("FlowTaskId")]
    public virtual FlowTaskDbModel FlowTask { get; set; } = null!;
}
