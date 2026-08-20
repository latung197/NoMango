using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Events;

public record FlowStarted(string TaskId, object? Meta = null);
public record FlowCompleted(string TaskId);
public record FlowAborted(string TaskId, string Reason);
public record FlowPaused(string TaskId, string Reason);
public record FlowResumed(string TaskId);
public record FlowChangeStep(string TaskId, FlowStep Step, Robot Robot, Station FromStation, Station ToStation);
public record FlowErrorOccurred(string TaskId, FlowStep Step, string Reason, Exception? Exception = null);


// public record StepScheduled(string TaskId, FlowStep Step);
// public record StepStarted(string TaskId);
// public record StepWaiting(string TaskId);
// public record StepResumed(string TaskId);
// public record StepCompleted(string TaskId, FlowStep Step);
// public record StepFailed(string TaskId, string Reason);
// public record StepTimedOut(string TaskId);

public record RetryScheduled(string TaskId, FlowStep Step, int Attempt, TimeSpan Delay);
public record RetryExhausted(string TaskId, FlowStep Step);

public record TimeoutFired(string TaskId, FlowStep Step);

public record ResourceReserved(string TaskId, string ResourceType, string ResourceId);
public record ResourceReleased(string TaskId, string ResourceType, string ResourceId);


public enum StopScope { Global, Flow }
public record StopEmergency(string TaskId, string Reason, StopScope Scope = StopScope.Flow);
public record StopCycle(string TaskId, string Reason, StopScope Scope = StopScope.Flow);

public record BizError(
  string TaskId,
  string Code,
  string Message,
  string? FromStation = null,
  string? ToStation = null);

