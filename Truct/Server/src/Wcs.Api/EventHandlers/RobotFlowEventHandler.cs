using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Wcs.Api.RobotFlow.Flow;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.EventHandlers;

/// <summary>
/// Handler xử lý các domain events và trigger MainFlow
/// Sử dụng IServiceScopeFactory để tạo scope mới cho mỗi event vì MainFlow là scoped service
/// </summary>
public class RobotFlowEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<RobotFlowEventHandler> logger,
    ITaskEventBroadcaster broadcaster,
    TaskScopedLockService taskLocks) : ICapSubscribe
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<RobotFlowEventHandler> _logger = logger;
    private readonly ITaskEventBroadcaster _broadcaster = broadcaster;
    private readonly TaskScopedLockService _taskLocks = taskLocks;

    // ── helpers ──────────────────────────────────────────────────────────────

    private static string StepName(int step) =>
        Enum.IsDefined(typeof(FlowStep), step)
            ? ((FlowStep)step).ToString()
            : $"Step {step}";

    private static string StepName(FlowStep step) => step.ToString();

    private async Task RunFlow(string taskId, string eventName, string? detail,
        Func<MainFlow, CancellationToken, Task> action, CancellationToken ct)
    {
        await using var flowLock = await _taskLocks.AcquireAsync($"flow:{taskId}", ct);

        _logger.LogInformation("Xử lý {Event}: TaskId={TaskId} {Detail}", eventName, taskId, detail);
        _broadcaster.Broadcast(taskId, eventName, detail);

        using var scope = _serviceScopeFactory.CreateScope();
        var mainFlow = scope.ServiceProvider.GetRequiredService<MainFlow>();
        await action(mainFlow, ct);
    }

    // ── Curtain ───────────────────────────────────────────────────────────────

    [CapSubscribe("Wcs.Common.Events.CurtainOpened")]
    public Task HandleCurtainOpenedAsync(CurtainOpened @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "CurtainOpened", @event.StationCode,
            (f, c) => f.Handle(@event, c), ct);

    [CapSubscribe("Wcs.Common.Events.CurtainClosed")]
    public Task HandleCurtainClosedAsync(CurtainClosed @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "CurtainClosed", @event.StationCode,
            (f, c) => f.Handle(@event, c), ct);

    // ── Conveyor ──────────────────────────────────────────────────────────────

    [CapSubscribe("Wcs.Common.Events.ConveyorBoxArrived")]
    public Task HandleConveyorBoxArrivedAsync(ConveyorBoxArrived @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "ConveyorBoxArrived", @event.StationCode,
            (f, c) => f.Handle(@event, c), ct);

    [CapSubscribe("Wcs.Common.Events.ConveyorBoxRemoved")]
    public Task HandleConveyorBoxRemovedAsync(ConveyorBoxRemoved @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "ConveyorBoxRemoved", @event.StationCode,
            (f, c) => f.Handle(@event, c), ct);

    [CapSubscribe("Wcs.Common.Events.ConveyorRemainingSpace")]
    public Task HandleConveyorRemainingSpaceAsync(ConveyorRemainingSpace @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "ConveyorRemainingSpace", @event.StationCode,
            (f, c) => f.Handle(@event, c), ct);

    // ── Box ───────────────────────────────────────────────────────────────────

    [CapSubscribe("Wcs.Common.Events.BoxArrived")]
    public Task HandleBoxArrivedAsync(BoxArrived @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "BoxArrived", @event.StationCode,
            (f, c) => f.Handle(@event, c), ct);

    [CapSubscribe("Wcs.Common.Events.BoxRemoved")]
    public Task HandleBoxRemovedAsync(BoxRemoved @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "BoxRemoved", @event.StationCode,
            (f, c) => f.Handle(@event, c), ct);

    // ── Robot ─────────────────────────────────────────────────────────────────

    [CapSubscribe("Wcs.Common.Events.RobotTaskComplete")]
    public Task HandleRobotTaskCompleteAsync(RobotTaskComplete @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "RobotTaskComplete", StepName(@event.Step),
            (f, c) => f.Handle(@event, c), ct);

    [CapSubscribe("Wcs.Common.Events.RobotTaskCancel")]
    public Task HandleRobotTaskCancelAsync(RobotTaskCancel @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "RobotTaskCancel", StepName(@event.Step),
            (f, c) => f.Handle(@event, c), ct);

    // ── Flow lifecycle ────────────────────────────────────────────────────────

    [CapSubscribe("Wcs.Common.Events.FlowResumed")]
    public Task HandleFlowResumedAsync(FlowResumed @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "FlowResumed", null,
            (f, c) => f.Handle(@event, c), ct);

    [CapSubscribe("Wcs.Common.Events.FlowStarted")]
    public Task HandleFlowStartedAsync(FlowStarted @event, CancellationToken ct) =>
        RunFlow(@event.TaskId, "FlowStarted", null,
            (f, c) => f.Handle(@event, c), ct);

    // ── Error / Stop ──────────────────────────────────────────────────────────

    [CapSubscribe("Wcs.Common.Events.FlowErrorOccurred")]
    public async Task HandleFlowErrorOccurredAsync(FlowErrorOccurred @event, CancellationToken ct)
    {
        _logger.LogInformation("Xử lý FlowErrorOccurred: TaskId={TaskId} Step={Step} Reason={Reason}",
            @event.TaskId, @event.Step, @event.Reason);
        _broadcaster.Broadcast(@event.TaskId, "FlowErrorOccurred",
            $"{StepName(@event.Step)} – {@event.Reason}");

        using var scope = _serviceScopeFactory.CreateScope();
        var errorFlow = scope.ServiceProvider.GetRequiredService<ErrorFlow>();
        await errorFlow.Handle(@event);
    }

    [CapSubscribe("Wcs.Common.Events.StopEmergency")]
    public async Task HandleStopEmergencyAsync(StopEmergency @event, CancellationToken ct)
    {
        _logger.LogInformation("Xử lý StopEmergency: TaskId={TaskId} Reason={Reason}",
            @event.TaskId, @event.Reason);
        _broadcaster.Broadcast(@event.TaskId, "StopEmergency", @event.Reason);

        using var scope = _serviceScopeFactory.CreateScope();
        var stopFlow = scope.ServiceProvider.GetRequiredService<StopFlow>();
        await stopFlow.Handle(@event);
    }

    [CapSubscribe("Wcs.Common.Events.TimeoutFired")]
    public async Task HandleTimeoutFiredAsync(TimeoutFired @event, CancellationToken ct)
    {
        _logger.LogInformation("Xử lý TimeoutFired: TaskId={TaskId} Step={Step}",
            @event.TaskId, @event.Step);
        _broadcaster.Broadcast(@event.TaskId, "TimeoutFired", StepName(@event.Step));

        using var scope = _serviceScopeFactory.CreateScope();
        var errorFlow = scope.ServiceProvider.GetRequiredService<ErrorFlow>();
        await errorFlow.Handle(@event);
    }
}
