using Wcs.Api.Controllers.DTOs;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.Extensions;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Services;

public class StationSnapshotService(
    StationService stationService,
    IFlowTaskRepository flowTaskRepository,
    ILogger<StationSnapshotService> logger)
{
    private readonly StationService _stationService = stationService;
    private readonly IFlowTaskRepository _flowTaskRepository = flowTaskRepository;
    private readonly ILogger<StationSnapshotService> _logger = logger;

    public async Task<StationSnapshot?> BuildAsync(string stationCode, CancellationToken cancellationToken = default)
    {
        var station = await _stationService.GetStationByCode(stationCode);
        if (station == null)
            return null;

        return await BuildAsync(station, cancellationToken);
    }

    public async Task<IReadOnlyList<StationSnapshot>> BuildByStageAsync(string stageCode, CancellationToken cancellationToken = default)
    {
        var stations = await _stationService.GetAllStations();
        var filtered = stations.Where(s => string.Equals(s.StageCode, stageCode, StringComparison.OrdinalIgnoreCase));
        var snapshots = new List<StationSnapshot>();

        foreach (var station in filtered)
        {
            var snapshot = await BuildAsync(station, cancellationToken);
            if (snapshot != null)
                snapshots.Add(snapshot);
        }

        return snapshots;
    }

    public async Task<StationSnapshot> BuildAsync(Station station, CancellationToken cancellationToken = default)
    {
        var profile = DetectProfile(station);
        var capabilities = BuildCapabilities(station);
        var opcTags = await _stationService.GetStationOpcTagsAsync(station);
        var tagLookup = opcTags.Tags.ToDictionary(t => t.TagType, StringComparer.Ordinal);

        var tray = BuildTraySnapshot(station, profile, tagLookup);
        var curtain = BuildCurtainSnapshot(station, tagLookup);
        var control = BuildControlSnapshot(station, tagLookup);
        var status = BuildStatusSnapshot(station, tagLookup);
        var conveyor = BuildConveyorSnapshot(station, profile, tagLookup);

        var sensorState = DeriveSensorState(profile, tray, conveyor);
        if (!IsPlcRunning(status))
            sensorState = StationDisplayState.Disconnect;

        var activeTask = await GetActiveTaskForStationAsync(station.Code, cancellationToken);
        var flow = BuildFlowSnapshot(activeTask);
        var displayState = DeriveDisplayState(station, sensorState, activeTask);
        if (!IsPlcRunning(status))
            displayState = StationDisplayState.Disconnect;

        return new StationSnapshot(
            station.Code,
            station.StageCode,
            profile,
            station.IsActive,
            capabilities,
            tray,
            curtain,
            control,
            status,
            conveyor,
            sensorState,
            displayState,
            flow,
            DateTime.UtcNow);
    }

    internal static StationTagProfile DetectProfile(Station station) =>
        station.HasConveyor
            ? StationTagProfile.ConveyorWithCurtain
            : station.HasCurtain && !string.IsNullOrEmpty(station.Tag_CurtainState)
                ? StationTagProfile.WithCurtain
                : StationTagProfile.Standard;

    private static StationSnapshotCapabilities BuildCapabilities(Station station) =>
        new(
            !string.IsNullOrEmpty(station.Tag_HasCassette),
            !string.IsNullOrEmpty(station.Tag_QRCode),
            station.HasCurtain && !string.IsNullOrEmpty(station.Tag_CurtainState),
            !string.IsNullOrEmpty(station.Tag_Control),
            !string.IsNullOrEmpty(station.Tag_Status),
            station.HasConveyor && !string.IsNullOrEmpty(station.Tag_ConveyorState),
            station.HasConveyor && !string.IsNullOrEmpty(station.Tag_ConveyorNumber));

    private static StationTraySnapshot BuildTraySnapshot(
        Station station,
        StationTagProfile profile,
        IReadOnlyDictionary<string, StationOpcTagItem> tagLookup)
    {
        if (profile == StationTagProfile.ConveyorWithCurtain)
        {
            var qr = TryGetTag(tagLookup, "QRCode");
            return new StationTraySnapshot(
                IsConfigured: !string.IsNullOrEmpty(station.Tag_QRCode),
                OpcGood: qr?.IsGood,
                HasCassette: null,
                QrCode: qr is { IsGood: true } ? qr.Value?.ToStringValue() : null);
        }

        var hasCassetteTag = TryGetTag(tagLookup, "HasCassette");
        var qrCodeTag = TryGetTag(tagLookup, "QRCode");

        StationHasCassette? hasCassette = null;
        if (hasCassetteTag is { IsGood: true })
        {
            hasCassette = (StationHasCassette)hasCassetteTag.Value.ToInt16OrDefault();
        }

        return new StationTraySnapshot(
            IsConfigured: !string.IsNullOrEmpty(station.Tag_HasCassette),
            OpcGood: hasCassetteTag?.IsGood,
            HasCassette: hasCassette,
            QrCode: qrCodeTag is { IsGood: true } ? qrCodeTag.Value?.ToStringValue() : null);
    }

    private static StationCurtainSnapshot BuildCurtainSnapshot(
        Station station,
        IReadOnlyDictionary<string, StationOpcTagItem> tagLookup)
    {
        var configured = station.HasCurtain && !string.IsNullOrEmpty(station.Tag_CurtainState);
        if (!configured)
        {
            return new StationCurtainSnapshot(false, null, null, null, null);
        }

        var tag = TryGetTag(tagLookup, "CurtainState");
        CurtainState? state = tag is { IsGood: true }
            ? (CurtainState)tag.Value.ToInt16OrDefault()
            : null;

        return new StationCurtainSnapshot(
            true,
            tag?.IsGood,
            state,
            state == CurtainState.IntrusionPossible,
            state == CurtainState.IntrusionProhibited);
    }

    private static StationControlSnapshot BuildControlSnapshot(
        Station station,
        IReadOnlyDictionary<string, StationOpcTagItem> tagLookup)
    {
        if (string.IsNullOrEmpty(station.Tag_Control))
            return new StationControlSnapshot(false, null, null);

        var tag = TryGetTag(tagLookup, "Control");
        StationControl? state = tag is { IsGood: true }
            ? (StationControl)tag.Value.ToInt16OrDefault()
            : null;

        return new StationControlSnapshot(true, tag?.IsGood, state);
    }

    private static StationStatusSnapshot BuildStatusSnapshot(
        Station station,
        IReadOnlyDictionary<string, StationOpcTagItem> tagLookup)
    {
        if (string.IsNullOrEmpty(station.Tag_Status))
            return new StationStatusSnapshot(false, null, null, null);

        var tag = TryGetTag(tagLookup, "Status");
        StationStatus? state = tag is { IsGood: true }
            ? (StationStatus)tag.Value.ToInt16OrDefault()
            : null;

        bool? isRunning = tag switch
        {
            { IsGood: true } => state == StationStatus.Running,
            not null => false,
            _ => null
        };

        return new StationStatusSnapshot(true, tag?.IsGood, state, isRunning);
    }

    internal static bool IsPlcRunning(StationStatusSnapshot status)
    {
        if (!status.IsConfigured)
            return true;

        return status.OpcGood == true && status.State == StationStatus.Running;
    }

    private static StationConveyorSnapshot BuildConveyorSnapshot(
        Station station,
        StationTagProfile profile,
        IReadOnlyDictionary<string, StationOpcTagItem> tagLookup)
    {
        if (profile != StationTagProfile.ConveyorWithCurtain)
        {
            return new StationConveyorSnapshot(false, null, null, null, null, ConveyorConstants.CapacityMax, null, null, null);
        }

        var stageTag = TryGetTag(tagLookup, "ConveyorState");
        ConveyorStage? stage = stageTag is { IsGood: true }
            ? (ConveyorStage)stageTag.Value.ToInt16OrDefault()
            : null;

        var numberConfigured = !string.IsNullOrEmpty(station.Tag_ConveyorNumber);
        var numberTag = numberConfigured ? TryGetTag(tagLookup, "ConveyorNumber") : null;
        int? itemCount = null;
        if (numberTag is { IsGood: true })
        {
            itemCount = numberTag.Value.ToInt16OrDefault();
            if (itemCount > ConveyorConstants.CapacityMax)
            {
                itemCount = ConveyorConstants.CapacityMax;
            }
        }

        var canAcceptDrop = DeriveCanAcceptDrop(stage, itemCount, numberTag?.IsGood);
        bool? isFull = itemCount.HasValue && numberTag?.IsGood == true
            ? itemCount >= ConveyorConstants.CapacityMax
            : null;
        bool? hasItemsOnBelt = itemCount.HasValue ? itemCount.Value > 0 : null;

        return new StationConveyorSnapshot(
            IsConfigured: numberConfigured || !string.IsNullOrEmpty(station.Tag_ConveyorState),
            StageOpcGood: stageTag?.IsGood,
            Stage: stage,
            NumberOpcGood: numberTag?.IsGood,
            ItemCount: itemCount,
            CapacityMax: ConveyorConstants.CapacityMax,
            CanAcceptDrop: canAcceptDrop,
            IsFull: isFull,
            HasItemsOnBelt: hasItemsOnBelt);
    }

    internal static bool? DeriveCanAcceptDrop(ConveyorStage? stage, int? conveyorNumber, bool? numberOpcGood)
    {
        if (stage == ConveyorStage.HasRequestPickup || stage == ConveyorStage.HasCassette)
            return false;

        if (stage != ConveyorStage.Empty)
            return null;

        if (conveyorNumber is null || numberOpcGood != true)
            return null;

        return conveyorNumber < ConveyorConstants.CapacityMax;
    }

    internal static StationDisplayState DeriveSensorState(
        StationTagProfile profile,
        StationTraySnapshot tray,
        StationConveyorSnapshot conveyor)
    {
        if (profile == StationTagProfile.ConveyorWithCurtain)
        {
            if (conveyor.IsFull == true)
                return StationDisplayState.Full;

            if (conveyor.StageOpcGood == true && conveyor.Stage.HasValue)
            {
                return conveyor.Stage.Value switch
                {
                    ConveyorStage.HasCassette => StationDisplayState.HasCassette,
                    ConveyorStage.Empty => StationDisplayState.NoCassette,
                    ConveyorStage.HasRequestPickup => StationDisplayState.Busy,
                    _ => StationDisplayState.Disconnect
                };
            }

            if (conveyor.NumberOpcGood == true && conveyor.ItemCount.HasValue)
            {
                return conveyor.ItemCount > 0
                    ? StationDisplayState.HasCassette
                    : StationDisplayState.NoCassette;
            }

            return StationDisplayState.Disconnect;
        }

        if (!tray.IsConfigured)
            return StationDisplayState.Disconnect;

        if (tray.OpcGood != true)
            return StationDisplayState.Disconnect;

        return tray.HasCassette == StationHasCassette.Exist
            ? StationDisplayState.HasCassette
            : StationDisplayState.NoCassette;
    }

    internal static StationDisplayState DeriveDisplayState(
        Station station,
        StationDisplayState sensorState,
        FlowTask? activeTask)
    {
        if (!station.IsActive)
            return StationDisplayState.Inactive;

        if (activeTask != null)
        {
            if (activeTask.FromStation.Code == station.Code
                && activeTask.CurrentStep < FlowStep.AfterPickup)
            {
                return StationDisplayState.Exporting;
            }

            if (activeTask.ToStation.Code == station.Code
                && activeTask.CurrentStep < FlowStep.BackToDropWaitingPoint)
            {
                return StationDisplayState.Receiving;
            }
        }

        return sensorState;
    }

    private static StationFlowSnapshot BuildFlowSnapshot(FlowTask? activeTask)
    {
        if (activeTask == null)
            return new StationFlowSnapshot(null, null, null, null);

        return new StationFlowSnapshot(
            activeTask.Id,
            (int)activeTask.CurrentStep,
            activeTask.FromStation.Code,
            activeTask.ToStation.Code);
    }

    private async Task<FlowTask?> GetActiveTaskForStationAsync(string stationCode, CancellationToken cancellationToken)
    {
        var tasks = await _flowTaskRepository.GetTasksInProgressOnStationAsync(stationCode, cancellationToken);
        return tasks.FirstOrDefault();
    }

    private static StationOpcTagItem? TryGetTag(IReadOnlyDictionary<string, StationOpcTagItem> tagLookup, string tagType) =>
        tagLookup.TryGetValue(tagType, out var tag) ? tag : null;

    public bool IsConfiguredStationTag(Station station, string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId))
            return false;

        return nodeId == station.Tag_HasCassette
            || nodeId == station.Tag_QRCode
            || nodeId == station.Tag_Control
            || nodeId == station.Tag_Status
            || nodeId == station.Tag_CurtainState
            || nodeId == station.Tag_ConveyorState
            || nodeId == station.Tag_ConveyorNumber;
    }
}
