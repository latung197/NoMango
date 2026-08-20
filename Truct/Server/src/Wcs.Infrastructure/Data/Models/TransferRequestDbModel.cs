using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Data.Models;

[Table("TransferRequests")]
public class TransferRequestDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public TransferRequestType Type { get; set; }

    [Required]
    [MaxLength(50)]
    public string FromStageCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ToStageCode { get; set; } = string.Empty;

    public Size Size { get; set; }

    [MaxLength(50)]
    public string? FromStationCode { get; set; }

    [MaxLength(50)]
    public string? ToStationCode { get; set; }

    public bool IsEmptyTray { get; set; }

    public bool IsWipTray { get; set; }

    public WarehousePendingKind WarehousePendingKind { get; set; } = WarehousePendingKind.None;

    [MaxLength(50)]
    public string? PendingWarehouseStationCode { get; set; }

    [MaxLength(50)]
    public string? CassetteCode { get; set; }

    [MaxLength(100)]
    public string? Product { get; set; }

    public int? Quantity { get; set; }

    [MaxLength(50)]
    public string? StorageStageCode { get; set; }

    [MaxLength(50)]
    public string? RobotCode { get; set; }

    public string? CranePositionsJson { get; set; }

    public TransferRequestSource Source { get; set; } = TransferRequestSource.Manual;

    public TransferRequestStatus Status { get; set; } = TransferRequestStatus.Waiting;

    public Guid? MatchedRequestId { get; set; }

    [MaxLength(50)]
    public string? FlowTaskId { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
