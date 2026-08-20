using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Api.RobotFlow;
using Wcs.Api.RobotFlow.UseCases;
using Wcs.Common.Constants;
using Wcs.Common.Services;
using Wcs.Rcs.Contracts;
using Wcs.Rcs.DTOs;
using Wcs.Rcs;
using Wcs.Api.Services;
using Wcs.Api.Configs;
using Microsoft.Extensions.Options;

namespace Wcs.Api.RobotFlow.Flow;

public sealed class MainFlow(
  ILogger<MainFlow> logger, ITaskStateStore store, IEventPublisher publisher,
  IFlowTaskService flowTaskService,
  TaskExecutionGateService taskExecutionGate,
  MoveToPickupWaitingPointHandler moveToPickupWaitingPointHandler,
  BeforePickHandler beforePickHandler,
  EnterPickupPointHandler enterPickupPointHandler,
  BackToPickupWaitingPointHandler backToPickupWaitingPointHandler,
  MoveToDropWaitingPointHandler moveToDropWaitingPointHandler,
  BeforeDropHandler beforeDropHandler,
  EnterDropPointHandler enterDropPointHandler,
  BackToDropWaitingPointHandler backToDropWaitingPointHandler,
  EndFlowHandler endFlowHandler,
  LiftRackHandler liftRackHandler,
  PutdownRackHandler putdownRackHandler,
  AfterPickupHandler afterPickupHandler,
  AfterDropHandler afterDropHandler,
  IRcsClient rcsClient,
  RobotService robotService,
  IOptions<HikConfig> hikOptions
) {
  private readonly ILogger<MainFlow> _logger = logger;
  private readonly ITaskStateStore _store = store;
  private readonly IEventPublisher _publisher = publisher;
  private readonly IFlowTaskService _flowTaskService = flowTaskService;
  private readonly TaskExecutionGateService _taskExecutionGate = taskExecutionGate;
  private readonly IRcsClient _rcsClient = rcsClient;
  private readonly RobotService _robotService = robotService;
  private readonly HikConfig _hikConfig = hikOptions.Value;

  /// <summary>Đổi thành <c>true</c> để bật log trace debug MainFlow.</summary>
  private static readonly bool EnableTraceLog = true;

  private void TraceLog(string message, params object?[] args)
  {
    if (!EnableTraceLog) return;
    _logger.LogInformation(message, args);
  }

  private void TraceLogWarn(string message, params object?[] args)
  {
    if (!EnableTraceLog) return;
    _logger.LogWarning(message, args);
  }

  public async Task Handle<T>(T e, CancellationToken ct) where T : class
  {
    var eventName = typeof(T).Name;
    var taskId = GetTaskId(e);

    if (string.IsNullOrEmpty(taskId)) {
      TraceLogWarn("[MainFlow] Event {EventName} không có TaskId", eventName);
      return;
    }

    TraceLog("[MainFlow] Nhận event {EventName}, TaskId={TaskId}", eventName, taskId);
    await ProcessFlowEvent(taskId, eventName, e, ct);
    TraceLog("[MainFlow] Xử lý xong event {EventName}, TaskId={TaskId}", eventName, taskId);
  }

  private static string GetTaskId<T>(T e) where T : class
  {
    var taskIdProperty = typeof(T).GetProperty("TaskId");
    return taskIdProperty?.GetValue(e)?.ToString() ?? string.Empty;
  }

  // ========================= PROCESS ========================= //

  /// <summary>
  /// Logic: 
  /// 1. Check task có waiting chưa hoàn thành mà trùng với eventName thì đánh dấu
  /// 2. Nếu vẫn còn waiting khác chưa hoàn thành thì dừng process và chờ
  /// 3. Nếu toàn bộ waiting đã hoàn thành thì clear waiting và đi tới bước tiếp theo
  /// </summary>
  private async Task ProcessFlowEvent(string taskId, string eventName, object eventData, CancellationToken ct)
  {
    TraceLog("[MainFlow] ProcessFlowEvent — load task TaskId={TaskId}", taskId);
    var task = await _store.LoadAsync(taskId);
    if (task is null) {
      TraceLogWarn("[MainFlow] TaskId={TaskId} không tồn tại", taskId);
      return;
    }

    TraceLog(
      "[MainFlow] TaskId={TaskId}, Step={CurrentStep}, Status={Status}, Event={EventName}, WaitingFor={@WaitingFor}",
      taskId, task.CurrentStep, task.Status, eventName, task.WaitingFor);

    if (task.Status != FlowStatus.Active && task.Status != FlowStatus.Pending) {
      TraceLogWarn("[MainFlow] TaskId={TaskId} Status={Status}, bỏ qua event {EventName}", taskId, task.Status, eventName);
      return;
    }

    if (eventName == nameof(FlowStarted) && await _taskExecutionGate.IsPausedAsync(ct))
    {
      task.Status = FlowStatus.Pending;
      await _store.SaveAsync(task);
      TraceLogWarn("[MainFlow] Task execution paused — TaskId={TaskId} giữ Pending, chờ resume", taskId);
      return;
    }

    task.Status = FlowStatus.Active;

    // Nhánh chung cho mọi loại reroute: khi đang có RerouteRequest và HIK báo AMR đã tới
    // điểm reroute (đúng leg mong đợi), chạy lại step được chỉ định (ResumeStep).
    // (Phải đặt TRƯỚC nhánh filler vì cú reroute cũng tiêu thụ một filler leg.)
    if (eventData is RobotTaskComplete rerouteArrival
        && task.Reroute is { } reroute
        && rerouteArrival.Step == reroute.ExpectedHikLeg)
    {
      TraceLog(
        "[MainFlow] TaskId={TaskId} — reroute arrival (HIK leg {Leg}), resume tại {ResumeStep}",
        taskId, reroute.ExpectedHikLeg, reroute.ResumeStep);
      task.Reroute = null;
      await RunStepHandler(task, reroute.ResumeStep, ct);
      return;
    }

    // Nhánh chung cho "filler leg" (leg đệm HIK tại drop waiting, vd 71): không có FlowStep,
    // chỉ tiêu thụ leg dư của template. Tới đủ số filler thì đi tiếp tới EnterDropPoint.
    if (eventData is RobotTaskComplete fillerArrival
        && task.CurrentStep == FlowStep.BeforeDrop
        && HikCallbackStep.IsFiller(fillerArrival.Step)
        && task.ConsumedDropFillers < _hikConfig.DropFillerCount)
    {
      task.ConsumedDropFillers++;
      task.SetWaitingConditions();
      await _store.SaveAsync(task);
      TraceLog(
        "[MainFlow] TaskId={TaskId} — filler leg {Leg} xong ({Consumed}/{Total})",
        taskId, fillerArrival.Step, task.ConsumedDropFillers, _hikConfig.DropFillerCount);
      await ProceedToNextStep(task, ct);
      return;
    }

    try {

      if (!ValidateFlowEvent(task, eventName, eventData)) {
        TraceLogWarn("[MainFlow] Validate thất bại — TaskId={TaskId}, Step={Step}, Event={EventName}", taskId, task.CurrentStep, eventName);
        return;
      }

      if (task.WaitingFor is null) {
        TraceLog("[MainFlow] TaskId={TaskId} — không có WaitingFor, gọi ProceedToNextStep", taskId);
        await ProceedToNextStep(task, ct);
      } else if (task.WaitingFor.IsAllMet) {
        TraceLog("[MainFlow] TaskId={TaskId} — WaitingFor đã đủ từ trước, clear và ProceedToNextStep", taskId);
        task.SetWaitingConditions();
        await _store.SaveAsync(task);
        await ProceedToNextStep(task, ct);
      } else {
        TraceLog("[MainFlow] TaskId={TaskId} — mark waiting cho event {EventName}", taskId, eventName);
        task.MarkWaitingConditionMet(eventName);
        await _store.SaveAsync(task);

        if (task.WaitingFor.IsAllMet) {
          TraceLog("[MainFlow] TaskId={TaskId} — đủ waiting sau mark, clear và ProceedToNextStep", taskId);
          task.SetWaitingConditions();
          await _store.SaveAsync(task);
          await ProceedToNextStep(task, ct);
        } else {
          TraceLog(
            "[MainFlow] TaskId={TaskId} — vẫn chờ điều kiện khác, dừng tại Step={Step}, WaitingFor={@WaitingFor}",
            taskId, task.CurrentStep, task.WaitingFor);
        }
      }
    } catch (Exception ex) {
      _logger.LogError(ex, "[MainFlow] Lỗi ProcessFlowEvent — TaskId={TaskId}, Step={Step}, Event={EventName}", taskId, task.CurrentStep, eventName);
      // Không truyền Exception vào event — CAP/System.Text.Json không serialize được TargetSite.
      await _publisher.PublishAsync(new FlowErrorOccurred(taskId, task.CurrentStep, ex.Message), ct);
    }
  }

  private bool ValidateFlowEvent(FlowTask task, string eventName, object eventData)
  {
    if (!FlowConstant.StepAllowEvents[task.CurrentStep].Contains(eventName))
    {
      _logger.LogWarning("Event {EventName} is not allowed for step {CurrentStep}", eventName, task.CurrentStep);
      return false;
    }

    return eventName switch
    {
      nameof(CurtainOpened) => _flowTaskService.ValidateCurtainEvent(task, (CurtainOpened)eventData),
      nameof(RobotTaskComplete) => _flowTaskService.ValidateRobotTaskCompleteEvent(task, (RobotTaskComplete)eventData),
      nameof(ConveyorBoxArrived) => _flowTaskService.ValidateConveyorBoxArrivedEvent(task, (ConveyorBoxArrived)eventData),
      nameof(ConveyorBoxRemoved) => _flowTaskService.ValidateConveyorBoxRemovedEvent(task, (ConveyorBoxRemoved)eventData),
      nameof(BoxArrived) => _flowTaskService.ValidateBoxArrivedEvent(task, (BoxArrived)eventData),
      nameof(BoxRemoved) => _flowTaskService.ValidateBoxRemovedEvent(task, (BoxRemoved)eventData),
      nameof(ConveyorRemainingSpace) => _flowTaskService.ValidateConveyorRemainingSpaceEvent(task, (ConveyorRemainingSpace)eventData),
      nameof(FlowStarted) => _flowTaskService.ValidateFlowStartedEvent(task, (FlowStarted)eventData),
      _ => true
    };
  }

  /// <summary>
  /// Tiến tới bước tiếp theo dựa trên current step
  /// </summary>
  private async Task ProceedToNextStep(FlowTask task, CancellationToken ct)
  {
    // Tại BeforeDrop: trước khi vào EnterDropPoint phải tiêu thụ hết "filler leg" của template HIK.
    // Mỗi filler là một continueTask tới drop waiting point; HIK sẽ bắn *_Complete (vd 71) cho leg đó.
    // Case redirect đã tiêu thụ trước 1 filler để di chuyển sang trạm mới nên ConsumedDropFillers
    // sẽ tự khớp và không bơm thừa.
    if (task.CurrentStep == FlowStep.BeforeDrop
        && task.ConsumedDropFillers < _hikConfig.DropFillerCount)
    {
      await IssueDropFiller(task, ct);
      return;
    }

    var nextStep = _flowTaskService.GetNextStep(task.CurrentStep);

    TraceLog(
      "[MainFlow] ProceedToNextStep — TaskId={TaskId}, CurrentStep={CurrentStep}, NextStep={NextStep}",
      task.Id, task.CurrentStep, nextStep?.ToString() ?? "(null/end)");

    if (task.RcsTaskId is not null && task.Robot is null) {
      TraceLog("[MainFlow] TaskId={TaskId} — query RCS Agv, RcsTaskId={RcsTaskId}", task.Id, task.RcsTaskId);
      var rcsTask = await _rcsClient.QueryTaskStatus(new QueryTaskStatusRequest(new List<string> { task.RcsTaskId }), ct);
      var taskData = rcsTask.Data?.FirstOrDefault(t => t.TaskCode == task.RcsTaskId);
      if (taskData is not null) {
        TraceLog("[MainFlow] TaskId={TaskId} — RCS AgvCode={AgvCode}", task.Id, taskData.AgvCode);
        if (taskData.AgvCode is not null) {
          var robot = await _robotService.GetRobotByCode(taskData.AgvCode, ct);
          if (robot is not null) {
            task.Robot = robot;
            TraceLog("[MainFlow] TaskId={TaskId} — gán Robot={RobotCode}", task.Id, robot.Code);
          }
        }
      } else {
        TraceLogWarn("[MainFlow] TaskId={TaskId} — RCS không trả task data", task.Id);
      }
    }

    await _store.SaveAsync(task);

    if (nextStep is not null) {
      TraceLog("[MainFlow] TaskId={TaskId} — publish FlowChangeStep → {NextStep}", task.Id, nextStep);
      await _publisher.PublishAsync(new FlowChangeStep(task.Id.ToString(), nextStep.Value, task.Robot, task.FromStation, task.ToStation), ct);
    }

    await RunStepHandler(task, nextStep, ct);
  }

  /// <summary>
  /// Bơm một "filler leg" HIK: continueTask tới drop waiting point để tiêu thụ leg đệm của template.
  /// Giữ nguyên CurrentStep=BeforeDrop; completion (vd 71_Complete) được xử lý bởi nhánh filler.
  /// </summary>
  private async Task IssueDropFiller(FlowTask task, CancellationToken ct)
  {
    var waitingPoint = task.ToStation.WaitingPoint;
    TraceLog(
      "[MainFlow] TaskId={TaskId} — bơm filler leg → WaitingPoint={WaitingPoint} ({Consumed}/{Total})",
      task.Id, waitingPoint, task.ConsumedDropFillers, _hikConfig.DropFillerCount);

    var result = await _rcsClient.ContinueTask(new ContinueTaskRequest(
      task.RcsTaskId ?? throw new InvalidDataException("RCS task ID is null"),
      RcsPositions.Berth(waitingPoint)
    ), ct);

    if (!result.Success) {
      await FlowTaskApiErrorLogger.LogRcsAsync(_store, task, "ContinueTask (filler leg)", result.Code, result.Message, ct);
      throw new InvalidDataException($"Failed to continue RCS filler leg: {result.Message}");
    }

    // Chờ RobotTaskComplete của filler; đặt WaitingFor để chặn event lạ kích hoạt bơm trùng.
    task.SetWaitingConditions(WaitingSet.For(nameof(RobotTaskComplete)));
    await _store.SaveAsync(task);
  }

  /// <summary>
  /// Gọi handler tương ứng với một step. Dùng chung cho luồng tiến bước bình thường
  /// (<see cref="ProceedToNextStep"/>) và luồng resume sau reroute.
  /// </summary>
  private async Task RunStepHandler(FlowTask task, FlowStep? nextStep, CancellationToken ct)
  {
    switch (nextStep) {
      case FlowStep.MoveToPickupWaitingPoint:
        TraceLog("[MainFlow] TaskId={TaskId} — handler MoveToPickupWaitingPoint", task.Id);
        await moveToPickupWaitingPointHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong MoveToPickupWaitingPoint", task.Id);
        break;

      case FlowStep.BeforePick:
        TraceLog("[MainFlow] TaskId={TaskId} — handler BeforePick", task.Id);
        await beforePickHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong BeforePick", task.Id);
        break;

      case FlowStep.EnterPickupPoint:
        TraceLog("[MainFlow] TaskId={TaskId} — handler EnterPickupPoint", task.Id);
        await enterPickupPointHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong EnterPickupPoint", task.Id);
        break;

      case FlowStep.LiftRack:
        TraceLog("[MainFlow] TaskId={TaskId} — handler LiftRack", task.Id);
        await liftRackHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong LiftRack", task.Id);
        break;

      case FlowStep.BackToPickupWaitingPoint:
        TraceLog("[MainFlow] TaskId={TaskId} — handler BackToPickupWaitingPoint", task.Id);
        await backToPickupWaitingPointHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong BackToPickupWaitingPoint", task.Id);
        break;

      case FlowStep.AfterPickup:
        TraceLog("[MainFlow] TaskId={TaskId} — handler AfterPickup", task.Id);
        await afterPickupHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong AfterPickup", task.Id);
        break;

      case FlowStep.MoveToDropWaitingPoint:
        TraceLog("[MainFlow] TaskId={TaskId} — handler MoveToDropWaitingPoint", task.Id);
        await moveToDropWaitingPointHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong MoveToDropWaitingPoint", task.Id);
        break;

      case FlowStep.BeforeDrop:
        TraceLog("[MainFlow] TaskId={TaskId} — handler BeforeDrop", task.Id);
        await beforeDropHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong BeforeDrop", task.Id);
        break;

      case FlowStep.EnterDropPoint:
        TraceLog("[MainFlow] TaskId={TaskId} — handler EnterDropPoint", task.Id);
        await enterDropPointHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong EnterDropPoint", task.Id);
        break;

      case FlowStep.PutdownRack:
        TraceLog("[MainFlow] TaskId={TaskId} — handler PutdownRack", task.Id);
        await putdownRackHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong PutdownRack", task.Id);
        break;

      case FlowStep.BackToDropWaitingPoint:
        TraceLog("[MainFlow] TaskId={TaskId} — handler BackToDropWaitingPoint", task.Id);
        await backToDropWaitingPointHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong BackToDropWaitingPoint", task.Id);
        break;

      case FlowStep.AfterDrop:
        TraceLog("[MainFlow] TaskId={TaskId} — handler AfterDrop", task.Id);
        await afterDropHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong AfterDrop", task.Id);
        break;

      case FlowStep.Completed:
        TraceLog("[MainFlow] TaskId={TaskId} — handler EndFlow (Completed)", task.Id);
        await endFlowHandler.Handle(task, ct);
        TraceLog("[MainFlow] TaskId={TaskId} — xong EndFlow", task.Id);
        break;

      default:
        TraceLogWarn("[MainFlow] TaskId={TaskId} — NextStep không hợp lệ: {NextStep}, CurrentStep={CurrentStep}", task.Id, nextStep, task.CurrentStep);
        break;
    }
  }
}
