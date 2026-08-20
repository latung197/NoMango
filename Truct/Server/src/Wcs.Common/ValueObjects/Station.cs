namespace Wcs.Common.ValueObjects;
using System.ComponentModel;

public enum InOut { 
  IN = 1,
  OUT = 2,
  ALL = IN | OUT
}

public enum Size {
  S = 1,
  M = 2,
  L = 3,
  XL = 4,
  LL = 5,
}

public enum ConveyorStage { 
  Empty = 1,
  HasRequestPickup = 2,
  HasCassette = 3,
}

public enum CurtainState { 
  IntrusionProhibited = 0,
  IntrusionPossible = 1
}

public enum StationStatus { 
  Offline = 0,
  Running = 1,
  Error = 2,
}

public enum StationAction { 
  None = 0,
  Export = 1,
  Import = 2
}

public enum StationControl { 
  Idle = 0,
  IntrusionRequest = 1,
  IntrusionInProgress = 2,
}

public enum StationHasCassette {
  NoExist = 0,
  Exist = 1,
}

public sealed class StationTag : StringValueObject<StationTag>
{
  public static readonly StationTag HasCassette = new("HasCassette");
  public static readonly StationTag CurtainState = new("CurtainState");
  public static readonly StationTag ConveyorState = new("ConveyorState");
  public static readonly StationTag ConveyorNumber = new("ConveyorNumber");
  public static readonly StationTag Control = new("Control");
  public static readonly StationTag Status = new("Status");
  public static readonly StationTag QRCode = new("QRCode");

  static StationTag()
  {
    // Register all static instances
    Register(HasCassette);
    Register(CurtainState);
    Register(ConveyorState);
    Register(ConveyorNumber);
    Register(Control);
    Register(Status);
    Register(QRCode);
  }

  private StationTag(string value) : base(value)
  {
  }
}