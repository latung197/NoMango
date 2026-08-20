namespace Wcs.Common.ValueObjects;

public enum FlowStep {
  Initial = 0,
  MoveToPickupWaitingPoint = 1,
  BeforePick = 2,
  EnterPickupPoint = 3,
  LiftRack = 4,
  BackToPickupWaitingPoint = 5,
  AfterPickup = 6,
  MoveToDropWaitingPoint = 7,
  BeforeDrop = 8,
  EnterDropPoint = 9,
  PutdownRack = 10,
  BackToDropWaitingPoint = 11,
  AfterDrop = 12,
  Completed = 13,
}


public enum FlowStatus {
  Pending = 0,
  Active = 1,
  Completed = 2,
  Paused = 3,
  Cancelled = 4,
  Error = 5,
}

public enum FlowPriority
{
    High = 1,
    Medium = 2,
    Low = 3
}
