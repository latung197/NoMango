namespace Wcs.Common.ValueObjects;

public enum RequestStep
{
  Initial = 0,
  Requested = 1,
  PickupStart = 2,
  PickupCompleted = 3,
  AMRCalled = 4,
  DeliverPrePoint = 5,
  Delivering = 6,
  MoveToPickupWaitingPoint = 7,
  BeforePick = 8,
  EnterPickupPoint = 9,
  LiftRack = 10,
  BackToPickupWaitingPoint = 11,
  AfterPickup = 12,
  MoveToDropWaitingPoint = 13,
  BeforeDrop = 14,
  EnterDropPoint = 15,
  PutdownRack = 16,
  BackToDropWaitingPoint = 17,
  AfterDrop = 18,
  Delivered = 19,
  Completed = 20,
}


public enum RequestStatus
{
  Pending = 0,
  Active = 1,
  Completed = 2,
  Paused = 3,
  Cancelled = 4,
  Error = 5,
}

public enum RequestPriority
{
    High = 1,
    Medium = 2,
    Low = 3
}
