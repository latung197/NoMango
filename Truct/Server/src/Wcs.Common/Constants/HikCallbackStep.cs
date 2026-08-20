using Wcs.Common.ValueObjects;

namespace Wcs.Common.Constants;

/// <summary>
/// Map webhook sequence (Method prefix) từ HIK → FlowStep WCS.
/// </summary>
public static class HikCallbackStep
{
    /// <summary>Leg đệm drop waiting trong template HIK (callback 71_Complete).</summary>
    public const int DropFillerLeg = 71;

    /// <summary>
    /// "Filler leg" (leg đệm) — leg HIK KHÔNG gắn FlowStep nghiệp vụ, chỉ để tiêu thụ leg dư
    /// trong template giữa BeforeDrop và EnterDropPoint. MainFlow tự xử lý generic, không có handler riêng.
    /// </summary>
    private static readonly HashSet<int> FillerLegs = [DropFillerLeg];

    private static readonly IReadOnlyDictionary<int, FlowStep> StepMap =
        new Dictionary<int, FlowStep>
        {
            [1] = FlowStep.MoveToPickupWaitingPoint,
            [3] = FlowStep.EnterPickupPoint,
            [4] = FlowStep.LiftRack,
            [5] = FlowStep.BackToPickupWaitingPoint,
            [7] = FlowStep.MoveToDropWaitingPoint,
            [9] = FlowStep.EnterDropPoint,
            [10] = FlowStep.PutdownRack,
            [11] = FlowStep.BackToDropWaitingPoint,
        };

    public static bool TryGetFlowStep(int hikStep, out FlowStep flowStep) =>
        StepMap.TryGetValue(hikStep, out flowStep);

    /// <summary>Leg này có phải filler/đệm (drop waiting buffer) không?</summary>
    public static bool IsFiller(int hikStep) => FillerLegs.Contains(hikStep);

    public static string GetDisplayName(int hikStep) =>
        IsFiller(hikStep) ? "DropFiller"
        : TryGetFlowStep(hikStep, out var step) ? step.ToString()
        : $"HIK step {hikStep}";
}
