using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class TransferRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public TransferRequestType Type { get; set; }
    public string FromStageCode { get; set; } = string.Empty;
    public string ToStageCode { get; set; } = string.Empty;
    public Size Size { get; set; }
    public string? FromStationCode { get; set; }
    public string? ToStationCode { get; set; }
    public bool IsEmptyTray { get; set; }
    public bool IsWipTray { get; set; }
    public WarehousePendingKind WarehousePendingKind { get; set; } = WarehousePendingKind.None;
    public string? PendingWarehouseStationCode { get; set; }
    public string? CassetteCode { get; set; }
    public string? Product { get; set; }
    public int? Quantity { get; set; }
    public string? StorageStageCode { get; set; }
    public string? RobotCode { get; set; }
    public List<CranePosition>? CranePositions { get; set; }
    public TransferRequestSource Source { get; set; } = TransferRequestSource.Manual;
    public TransferRequestStatus Status { get; set; } = TransferRequestStatus.Waiting;
    public Guid? MatchedRequestId { get; set; }
    public string? FlowTaskId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
