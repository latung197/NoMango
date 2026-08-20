using Wcs.Common.Entities;

namespace Wcs.Common.Events;

public record BoxArrived(string TaskId, string StationCode);
public record BoxRemoved(string TaskId, string StationCode);

public record StationChanged(Station Station);

public record TransferRequestChanged(TransferRequest Request);
