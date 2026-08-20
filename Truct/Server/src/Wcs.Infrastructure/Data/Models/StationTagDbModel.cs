using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("StationTags")]
public class StationTagDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public Guid StationId { get; set; }

    [Required]
    public StationTag TagType { get; set; } = StationTag.HasCassette;

    [Required]
    public string Tag { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    [ForeignKey("StationId")]
    public virtual StationDbModel Station { get; set; } = null!;
}
