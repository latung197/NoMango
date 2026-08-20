using Wcs.Common.ValueObjects;

namespace Wcs.Common.Entities;

public sealed class Station
{
    public Guid Id { get; set; }

    // Trạng thái cơ bản của 1 station
    public string Code { get; set; } = string.Empty;
    public string StageCode { get; set; } = string.Empty;
    public InOut Type { get; set; }
    public List<Size> Sizes { get; set; } = [];
    public string? Tag_HasCassette { get; set; }

    public string? Tag_Status { get; set; }
    public string? Tag_Control { get; set; }
    public string? Tag_QRCode { get; set; }

    // Rèm/cửa (nếu có)
    public bool HasCurtain { get; set; }
    public string? Tag_CurtainState { get; set; }

    // Băng tải (nếu có)
    public bool HasConveyor { get; set; }
    public string? Tag_ConveyorState { get; set; }
    public string? Tag_ConveyorNumber { get; set; }

    // Cửa kho (nếu có)
    public bool HasWarehouseDoor { get; set; }

    // Station dành cho nguyên vật liệu phụ
    public bool IsAuxiliaryMaterial { get; set; }

    // Sức chứa
    public int Capacity { get; set; }

    // Mapping point (computed properties)
    public string MainPoint { get; set; } = string.Empty;
    public string WaitingPoint { get; set; } = string.Empty;

    public bool AutoReceiveEnabled { get; set; }
    public bool AutoSendEnabled { get; set; }
    public bool AutoSendIsEmptyTray { get; set; }
    public string? AutoSendToStage { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsExistCassette { get; set; } = false;
    public StationStatus Status { get; set; } = StationStatus.Offline;
    public StationAction Action { get; set; } = StationAction.None;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Default constructor
    public Station() { }

    // Main constructor
    public Station(
        string code,
        string stageCode,
        InOut type,
        IEnumerable<Size> sizes,
        string? tag_HasCassette = null,
        string? tag_Status = null,
        string? tag_Control = null,
        string? tag_QRCode = null,
        bool hasCurtain = false,
        string? tag_CurtainState = null,
        bool hasConveyor = false,
        string? tag_ConveyorState = null,
        string mainPoint = "",
        string waitingPoint = "")
    {
        Id = Guid.NewGuid();
        Code = code;
        StageCode = stageCode;
        Type = type;
        Sizes = sizes.Distinct().OrderBy(s => (int)s).ToList();
        Tag_HasCassette = tag_HasCassette;
        Tag_Status = tag_Status;
        Tag_Control = tag_Control;
        Tag_QRCode = tag_QRCode;
        HasCurtain = hasCurtain;
        Tag_CurtainState = tag_CurtainState;
        HasConveyor = hasConveyor;
        Tag_ConveyorState = tag_ConveyorState;
        IsActive = true;
        MainPoint = mainPoint;
        WaitingPoint = waitingPoint;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business methods
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public bool SupportsSize(Size size) => Sizes.Contains(size);

    public bool CanAcceptPickup() => Type == InOut.IN || Type == InOut.ALL;
    public bool CanAcceptDrop() => Type == InOut.OUT || Type == InOut.ALL;
}
