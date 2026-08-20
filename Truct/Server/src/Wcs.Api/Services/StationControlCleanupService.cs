using Opc.Ua;
using Wcs.Common.Entities;
using Wcs.Common.Extensions;
using Wcs.Common.ValueObjects;
using Wcs.OpcUa.Contracts;

namespace Wcs.Api.Services;

public sealed class StationControlCleanupService(
    IOpcUaClient opcClient,
    ILogger<StationControlCleanupService> logger)
{
    private readonly IOpcUaClient _opcClient = opcClient;
    private readonly ILogger<StationControlCleanupService> _logger = logger;

    public async Task ResetControlTagsForTaskAsync(FlowTask task, string reason, CancellationToken ct = default)
    {
        foreach (var station in GetStationsToReset(task))
        {
            await ResetStationControlIfNonIdleAsync(station, reason, ct);
        }
    }

    public async Task ResetStationControlIfNonIdleAsync(Station station, string reason, CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(station.Tag_Control))
        {
            return;
        }

        var value = await _opcClient.ReadValueAsync(station.Tag_Control);
        if (!StatusCode.IsGood(value.StatusCode))
        {
            _logger.LogWarning(
                "[StationControlCleanup] Skip reset because OPC read failed. Station={StationCode}, Tag={Tag}, Status={Status}, Reason={Reason}",
                station.Code,
                station.Tag_Control,
                value.StatusCode,
                reason);
            return;
        }

        var currentValue = value.Value.ToInt32OrDefault();
        if (currentValue == (int)StationControl.Idle)
        {
            return;
        }

        var success = await _opcClient.WriteValueAsync(station.Tag_Control, (int)StationControl.Idle);
        if (!success)
        {
            _logger.LogWarning(
                "[StationControlCleanup] Failed to reset control tag. Station={StationCode}, Tag={Tag}, OldValue={OldValue}, Reason={Reason}",
                station.Code,
                station.Tag_Control,
                currentValue,
                reason);
            return;
        }

        _logger.LogWarning(
            "[StationControlCleanup] Reset control tag. Station={StationCode}, Tag={Tag}, OldValue={OldValue}, Reason={Reason}",
            station.Code,
            station.Tag_Control,
            currentValue,
            reason);
    }

    private static IEnumerable<Station> GetStationsToReset(FlowTask task)
    {
        return task.CurrentStep switch
        {
            FlowStep.BeforePick
                or FlowStep.EnterPickupPoint
                or FlowStep.LiftRack
                or FlowStep.BackToPickupWaitingPoint
                or FlowStep.AfterPickup => YieldIfConfigured(task.FromStation),

            FlowStep.BeforeDrop
                or FlowStep.EnterDropPoint
                or FlowStep.PutdownRack
                or FlowStep.BackToDropWaitingPoint
                or FlowStep.AfterDrop => YieldIfConfigured(task.ToStation),

            _ => []
        };
    }

    private static IEnumerable<Station> YieldIfConfigured(Station? station)
    {
        if (station is not null && !string.IsNullOrWhiteSpace(station.Tag_Control))
        {
            yield return station;
        }
    }
}
