using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wcs.Infrastructure.Data.Models;

public enum CraneTaskDispatchStatus
{
    Dispatching = 0,
    Accepted = 1,
    Completing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}

[Table("CraneTaskDispatches")]
public class CraneTaskDispatchDbModel
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public int TaskNo { get; set; }

    [MaxLength(50)]
    public string? FlowTaskId { get; set; }

    public int TaskType { get; set; }

    public CraneTaskDispatchStatus Status { get; set; } = CraneTaskDispatchStatus.Dispatching;

    [MaxLength(200)]
    public string IdempotencyKey { get; set; } = string.Empty;

    public int? CompleteStatus { get; set; }

    public int? ErrorCode { get; set; }

    [MaxLength(500)]
    public string? ErrorNote { get; set; }

    [MaxLength(500)]
    public string? LastMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletingAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }

    /// <summary>Giá trị D2451 đã ghi (1=QR OK, 2=không đọc được QR lúc D2030).</summary>
    public int? QrFinishedRaisedValue { get; set; }

    public DateTime? QrFinishedRaisedAt { get; set; }

    public DateTime? QrFinishedClearedAt { get; set; }

    /// <summary>Luôn true khi WCS ghi D2451=1 sau D2030 1→0.</summary>
    public bool? QrFinishedHadCachedQr { get; set; }
}
