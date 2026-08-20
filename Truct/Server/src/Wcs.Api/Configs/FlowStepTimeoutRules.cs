using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Configs;

/// <summary>
/// Quy tắc áp timeout theo step config và loại điều kiện đang chờ.
/// Chỉ timeout khi chờ handshake trạm (rèm, băng tải, cassette) — không timeout khi chờ AMR di chuyển.
/// </summary>
public static class FlowStepTimeoutRules
{
    private static readonly HashSet<string> HandshakeConditions = new(StringComparer.Ordinal)
    {
        nameof(CurtainOpened),
        nameof(ConveyorBoxArrived),
        nameof(ConveyorBoxRemoved),
        nameof(BoxArrived),
        nameof(BoxRemoved),
        nameof(ConveyorRemainingSpace),
    };

    public static bool IsWaitingForHandshake(WaitingSet? waitingFor)
    {
        if (waitingFor is null)
        {
            return false;
        }

        return waitingFor.GetNeeds().Any(HandshakeConditions.Contains);
    }

    public static bool ShouldApplyTimeout(FlowTask task, FlowTimeoutOptions options)
    {
        return options.IsTimeoutMonitored(task.CurrentStep, task.TaskSize)
            && IsWaitingForHandshake(task.WaitingFor);
    }
}
