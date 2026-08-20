using Wcs.Common.Entities;

namespace Wcs.Common.Events;

public record ConveyorReadyRequested(string TaskId, string StationCode);
public record ConveyorBoxArrived(string TaskId, string StationCode);
public record ConveyorBoxRemoved(string TaskId, string StationCode);
public record ConveyorTimeout(string TaskId, string StationCode);
public record ConveyorFull(string TaskId, string StationCode);
public record ConveyorRemainingSpace(string TaskId, string StationCode);