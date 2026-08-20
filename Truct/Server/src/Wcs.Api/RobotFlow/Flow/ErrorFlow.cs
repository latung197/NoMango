namespace Wcs.Api.RobotFlow.Flow;

using Microsoft.Extensions.Options;
using Wcs.Api.Configs;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

public sealed class ErrorFlow(
  ILogger<ErrorFlow> logger,
  ITaskStateStore store,
  IEventPublisher publisher,
  IAmrLoadStateService amrLoadState,
  DropStationRedirectService dropRedirect,
  IOptions<FlowTimeoutOptions> flowTimeoutOptions
) {
  private readonly ILogger<ErrorFlow> _logger = logger;
  private readonly ITaskStateStore _store = store;
  private readonly IEventPublisher _publisher = publisher;
  private readonly IAmrLoadStateService _amrLoadState = amrLoadState;
  private readonly DropStationRedirectService _dropRedirect = dropRedirect;
  private readonly FlowTimeoutOptions _flowTimeouts = flowTimeoutOptions.Value;
  
  public async Task Handle(FlowErrorOccurred e)
  {
    var task = await _store.LoadAsync(e.TaskId);
    if (task is null) {
      _logger.LogWarning("Task {TaskId} not found when processing flow error occurred", e.TaskId);
      return;
    }

    await _store.CancelTask(task.Id.ToString());
    await _store.LogAsync(task, e.Reason, nameof(FlowErrorOccurred));
  }

  public async Task Handle(TimeoutFired e)
  {
    var task = await _store.LoadAsync(e.TaskId);
    if (task is null) {
      _logger.LogWarning("Task {TaskId} not found when processing timeout fired", e.TaskId);
      return;
    }

    if (!FlowStepTimeoutRules.ShouldApplyTimeout(task, _flowTimeouts))
    {
      _logger.LogWarning(
        "Bỏ qua TimeoutFired tại step {Step} — không chờ handshake trạm (WaitingFor={@WaitingFor}). TaskId={TaskId}",
        task.CurrentStep, task.WaitingFor, task.Id);
      return;
    }

    var (errorCode, errorMessage) = ResolveTimeoutError(task);
    await _publisher.PublishAsync(new BizError(task.Id.ToString(), errorCode, errorMessage));
    if (task.CurrentStep == FlowStep.BeforeDrop
        && task.RerouteCount == 0
        && _amrLoadState.CanRedirectDrop(task))
    {
      var redirected = await _dropRedirect.TryRedirectAsync(task, CancellationToken.None);
      if (redirected)
      {
        return;
      }

      _logger.LogWarning(
        "Drop redirect failed — no available fallback station. TaskId={TaskId}, ToStation={ToStation}",
        task.Id, task.ToStation.Code);

      await _publisher.PublishAsync(new BizError(
        task.Id.ToString(),
        "E017",
        "Không thể tìm được trạm đặt thay thế"));
    }

    if (task.CurrentStep == FlowStep.BeforeDrop
        && task.RerouteCount == 0
        && _amrLoadState.CanRedirectDrop(task))
    {
      await _store.LogAsync(task, $"Timeout fired, no fallback station available: {errorMessage}", nameof(TimeoutFired));
      return;
    }

    await _store.CancelTask(e.TaskId);
    await _store.LogAsync(task, $"Timeout fired: {errorMessage}", nameof(TimeoutFired));
  }

  private static (string Code, string Message) ResolveTimeoutError(FlowTask task)
  {
    var needs = task.WaitingFor?.GetNeeds().ToHashSet() ?? [];

    if (needs.Contains(nameof(CurtainOpened))) {
      return task.CurrentStep switch {
        FlowStep.BeforePick => ("E002", "Timeout: curtain did not open before pick"),
        FlowStep.BeforeDrop => ("E001", "Timeout: curtain did not open before drop"),
        _ => ("E001", "Timeout fired, task cancelled")
      };
    }

    if (needs.Contains(nameof(ConveyorBoxArrived)) && task.CurrentStep == FlowStep.BeforePick) {
      return ("E004", "Timeout: conveyor box did not arrive before pick");
    }

    if (needs.Contains(nameof(ConveyorBoxRemoved)) && task.CurrentStep == FlowStep.BeforeDrop) {
      return ("E003", "Timeout: conveyor box was not removed before drop");
    }

    return ("E001", "Timeout fired, task cancelled");
  }
}
