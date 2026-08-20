using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Events;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StationTagProfile
{
    Standard,
    WithCurtain,
    ConveyorWithCurtain
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StationDisplayState
{
    NoCassette,
    HasCassette,
    Full,
    Busy,
    Exporting,
    Receiving,
    Disconnect,
    Inactive
}

public record StationSnapshotCapabilities(
    [property: JsonPropertyName("hasHasCassetteTag")] bool HasHasCassetteTag,
    [property: JsonPropertyName("hasQrcodeTag")] bool HasQrcodeTag,
    [property: JsonPropertyName("hasCurtainTag")] bool HasCurtainTag,
    [property: JsonPropertyName("hasControlTag")] bool HasControlTag,
    [property: JsonPropertyName("hasStatusTag")] bool HasStatusTag,
    [property: JsonPropertyName("hasConveyorStateTag")] bool HasConveyorStateTag,
    [property: JsonPropertyName("hasConveyorNumberTag")] bool HasConveyorNumberTag
);

public record StationStatusSnapshot(
    [property: JsonPropertyName("isConfigured")] bool IsConfigured,
    [property: JsonPropertyName("opcGood")] bool? OpcGood,
    [property: JsonPropertyName("state")] StationStatus? State,
    [property: JsonPropertyName("isRunning")] bool? IsRunning
);

public record StationTraySnapshot(
    [property: JsonPropertyName("isConfigured")] bool IsConfigured,
    [property: JsonPropertyName("opcGood")] bool? OpcGood,
    [property: JsonPropertyName("hasCassette")] StationHasCassette? HasCassette,
    [property: JsonPropertyName("qrCode")] string? QrCode
);

public record StationCurtainSnapshot(
    [property: JsonPropertyName("isConfigured")] bool IsConfigured,
    [property: JsonPropertyName("opcGood")] bool? OpcGood,
    [property: JsonPropertyName("state")] CurtainState? State,
    [property: JsonPropertyName("isOpen")] bool? IsOpen,
    [property: JsonPropertyName("isClosed")] bool? IsClosed
);

public record StationControlSnapshot(
    [property: JsonPropertyName("isConfigured")] bool IsConfigured,
    [property: JsonPropertyName("opcGood")] bool? OpcGood,
    [property: JsonPropertyName("state")] StationControl? State
);

public record StationConveyorSnapshot(
    [property: JsonPropertyName("isConfigured")] bool IsConfigured,
    [property: JsonPropertyName("stageOpcGood")] bool? StageOpcGood,
    [property: JsonPropertyName("stage")] ConveyorStage? Stage,
    [property: JsonPropertyName("numberOpcGood")] bool? NumberOpcGood,
    [property: JsonPropertyName("itemCount")] int? ItemCount,
    [property: JsonPropertyName("capacityMax")] int CapacityMax,
    [property: JsonPropertyName("canAcceptDrop")] bool? CanAcceptDrop,
    [property: JsonPropertyName("isFull")] bool? IsFull,
    [property: JsonPropertyName("hasItemsOnBelt")] bool? HasItemsOnBelt
);

public record StationFlowSnapshot(
    [property: JsonPropertyName("activeTaskId")] string? ActiveTaskId,
    [property: JsonPropertyName("currentStep")] int? CurrentStep,
    [property: JsonPropertyName("fromStationCode")] string? FromStationCode,
    [property: JsonPropertyName("toStationCode")] string? ToStationCode
);

public record StationSnapshot(
    [property: JsonPropertyName("stationCode")] string StationCode,
    [property: JsonPropertyName("stageCode")] string StageCode,
    [property: JsonPropertyName("profile")] StationTagProfile Profile,
    [property: JsonPropertyName("isActive")] bool IsActive,
    [property: JsonPropertyName("capabilities")] StationSnapshotCapabilities Capabilities,
    [property: JsonPropertyName("tray")] StationTraySnapshot Tray,
    [property: JsonPropertyName("curtain")] StationCurtainSnapshot Curtain,
    [property: JsonPropertyName("control")] StationControlSnapshot Control,
    [property: JsonPropertyName("status")] StationStatusSnapshot Status,
    [property: JsonPropertyName("conveyor")] StationConveyorSnapshot Conveyor,
    [property: JsonPropertyName("sensorState")] StationDisplayState SensorState,
    [property: JsonPropertyName("displayState")] StationDisplayState DisplayState,
    [property: JsonPropertyName("flow")] StationFlowSnapshot Flow,
    [property: JsonPropertyName("updatedAt")] DateTime UpdatedAt
);

public static class ConveyorConstants
{
    public const int CapacityMax = 5;
}

public record StationSnapshotChanged(StationSnapshot Snapshot);
