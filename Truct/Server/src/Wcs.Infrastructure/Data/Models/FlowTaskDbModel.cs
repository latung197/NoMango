using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("FlowTasks")]
public class FlowTaskDbModel
{
    [Key]
    [MaxLength(10)]
    public string Id { get; set; } = string.Empty;

    // Reference to config-based entities (stored as strings)
    [Required]
    [MaxLength(50)]
    public string FromStationCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ToStationCode { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? RobotCode { get; set; }

    public FlowStep CurrentStep { get; set; } = FlowStep.Initial;

    [MaxLength(100)]
    public string? RcsTaskId { get; set; }

    [Required]
    [MaxLength(20)]
    public FlowStatus Status { get; set; } = FlowStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }
    
    public DateTime? CurrentStepStartedAt { get; set; }

    [MaxLength(2000)]
    public string? CranePositionsJson { get; set; }

    [MaxLength(50)]
    public string? CassetteCode { get; set; }

    [MaxLength(50)]
    public string? StorageStageCode { get; set; }

    public int? Quantity { get; set; }

    [MaxLength(100)]
    public string? Product { get; set; }

    public int? CraneTaskNo { get; set; }

    [MaxLength(200)]
    public string? RerouteJson { get; set; }

    public int RerouteCount { get; set; }

    public int ConsumedDropFillers { get; set; }

    public int TaskSize { get; set; } = 1;

    // No navigation properties - Robot/Station loaded from config

    public virtual ICollection<FlowTaskWaitingSetDbModel> WaitingSets { get; set; } = [];
    public virtual ICollection<FlowTaskHistoryDbModel> History { get; set; } = [];
}
