using Wcs.Common.Entities;

namespace Wcs.Common.Events;

public record RobotTaskComplete(string TaskId, int Step);
public record RobotTaskBegin(string TaskId, int Step);
public record RobotTaskCancel(string TaskId, int Step);