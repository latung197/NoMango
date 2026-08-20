using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Services;

public sealed class AmrLoadStateService : IAmrLoadStateService
{
    /// <summary>
    /// Các bước sau khi lift rack (HIK 4_Complete) và trước khi hoàn tất putdown (HIK 10_Complete).
    /// </summary>
    private static readonly HashSet<FlowStep> CarryingSteps =
    [
        FlowStep.BackToPickupWaitingPoint,
        FlowStep.AfterPickup,
        FlowStep.MoveToDropWaitingPoint,
        FlowStep.BeforeDrop,
        FlowStep.EnterDropPoint,
        FlowStep.PutdownRack,
    ];

    public bool IsCarryingLoad(FlowTask task) => CarryingSteps.Contains(task.CurrentStep);

    public bool CanRedirectDrop(FlowTask task) =>
        IsCarryingLoad(task)
        && task.CurrentStep == FlowStep.BeforeDrop
        && !string.IsNullOrEmpty(task.RcsTaskId);
}
