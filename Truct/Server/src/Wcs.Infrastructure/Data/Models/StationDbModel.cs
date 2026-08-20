using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("Stations")]
public class StationDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string StageCode { get; set; } = string.Empty;

    [Required]
    public InOut Type { get; set; } = InOut.IN;

    [Required]
    [MaxLength(50)]
    public string Sizes { get; set; } = "1";

    [Required]
    public Boolean HasCurtain { get; set; } = false;

    [Required]
    public Boolean HasConveyor { get; set; } = false;

    [Required]
    public Boolean HasWarehouseDoor { get; set; } = false;

    [Required]
    public bool IsAuxiliaryMaterial { get; set; } = false;

    [Required]
    public int Capacity { get; set; } = 0;

    [Required]
    [MaxLength(50)]
    public string MainPoint { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string WaitingPoint { get; set; } = string.Empty;

    [Required]
    public bool AutoReceiveEnabled { get; set; } = false;

    [Required]
    public bool AutoSendEnabled { get; set; } = false;

    [Required]
    public bool AutoSendIsEmptyTray { get; set; } = false;

    [MaxLength(50)]
    public string? AutoSendToStage { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public virtual ICollection<StationTagDbModel> Tags { get; set; } = [];
}
