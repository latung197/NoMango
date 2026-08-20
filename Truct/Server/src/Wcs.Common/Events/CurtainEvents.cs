using Wcs.Common.Entities;

namespace Wcs.Common.Events;

public record CurtainOpenRequested(string TaskId, string StationCode);
public record CurtainOpened(string TaskId, string StationCode);
public record CurtainOpenTimeout(string TaskId, string StationCode);
public record CurtainClosed(string TaskId, string StationCode);
