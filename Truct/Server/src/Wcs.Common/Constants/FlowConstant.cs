using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Constants;

public static class FlowConstant
{
  /// <summary>Bước flow từ đó fromStation không còn bị coi là "occupied".</summary>
  public static readonly FlowStep FromStationReleaseStep = FlowStep.MoveToDropWaitingPoint;

  public static readonly Dictionary<FlowStep, string[]> StepAllowEvents =
    new()
    { 
        { FlowStep.Initial,  new[] { nameof(FlowStarted), nameof(FlowResumed) } },
        { FlowStep.MoveToPickupWaitingPoint,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.BeforePick,  new[] { 
          nameof(CurtainOpened), 
          nameof(ConveyorBoxArrived),
          nameof(BoxArrived),
          nameof(FlowResumed),
        } },
        { FlowStep.EnterPickupPoint,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.LiftRack,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.BackToPickupWaitingPoint,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.AfterPickup,  new[] { nameof(FlowResumed), nameof(FlowStarted) } },
        { FlowStep.MoveToDropWaitingPoint,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.BeforeDrop,  new[] { 
          nameof(CurtainOpened), 
          nameof(ConveyorBoxRemoved),
          nameof(BoxRemoved),
          nameof(ConveyorRemainingSpace), 
          nameof(FlowResumed)
        } },
        { FlowStep.EnterDropPoint,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.PutdownRack,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.BackToDropWaitingPoint,  new[] { nameof(RobotTaskComplete), nameof(FlowResumed) } },
        { FlowStep.AfterDrop,  new[] { nameof(FlowResumed) } },
        { FlowStep.Completed, [] }
    };

  public static readonly FlowStep[] FlowStepList = [
    FlowStep.Initial,
    FlowStep.MoveToPickupWaitingPoint,
    FlowStep.BeforePick,
    FlowStep.EnterPickupPoint,
    FlowStep.LiftRack,
    FlowStep.BackToPickupWaitingPoint,
    FlowStep.AfterPickup,
    FlowStep.MoveToDropWaitingPoint,
    FlowStep.BeforeDrop,
    FlowStep.EnterDropPoint,
    FlowStep.PutdownRack,
    FlowStep.BackToDropWaitingPoint,
    FlowStep.AfterDrop,
    FlowStep.Completed,
  ];
}