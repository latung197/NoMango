using DotNetCore.CAP;
using Wcs.Api.RobotFlow.UseCases;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data.Models;
using Wcs.Okamura;
using Wcs.Okamura.Enums;
using Wcs.Okamura.Events;
using Wcs.Okamura.Models;
using Wcs.Okamura.Services;
using CommonCranePosition = Wcs.Common.ValueObjects.CranePosition;
using OkamuraCranePosition = Wcs.Okamura.Models.CranePosition;

namespace Wcs.Api.EventHandlers;

public class CraneEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    IManualCraneTaskTracker manualCraneTaskTracker,
    ICraneTaskNoGenerator craneTaskNoGenerator,
    IEventPublisher publisher,
    ILogger<CraneEventHandler> logger) : ICapSubscribe
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IManualCraneTaskTracker _manualCraneTaskTracker = manualCraneTaskTracker;
    private readonly ICraneTaskNoGenerator _craneTaskNoGenerator = craneTaskNoGenerator;
    private readonly IEventPublisher _publisher = publisher;
    private readonly ILogger<CraneEventHandler> _logger = logger;

    [CapSubscribe("Wcs.Common.Events.RobotTaskBegin")]
    public async Task HandleRobotTaskBeginAsync(RobotTaskBegin @event, CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<WarehouseOutboundOnAgvStartHandler>();
        await handler.Handle(@event, ct);
    }

    [CapSubscribe("Wcs.Okamura.Events.CraneTaskCompletedEvent")]
    public async Task HandleCraneTaskCompletedAsync(CraneTaskCompletedEvent @event, CancellationToken ct)
    {
        if (@event.CompleteStatus is not (TaskCompleteStatus.AutoCompleted or TaskCompleteStatus.ManualCompleted))
        {
            _logger.LogWarning(
                "Skip WMS complete for crane task {TaskNo}; CompleteStatus={CompleteStatus}",
                @event.TaskNo,
                @event.CompleteStatus);
            return;
        }

        // Step 1: Resolve context. Automatic inbound has a FlowTask; manual inbound is tracked by task number.
        using var scope = _serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFlowTaskRepository>();
        var wmsService = scope.ServiceProvider.GetRequiredService<IWmsService>();
        var dispatchService = scope.ServiceProvider.GetRequiredService<CraneTaskDispatchService>();

        var dispatch = await dispatchService.GetByTaskNoAsync(@event.TaskNo, ct);
        if (dispatch is not null)
        {
            var shouldHandle = await dispatchService.TryBeginCompletionAsync(
                @event.TaskNo,
                @event.CompleteStatus,
                @event.ErrorCode,
                @event.ErrorNote,
                ct);

            if (!shouldHandle)
            {
                return;
            }
        }

        FlowTask? flowTask = null;
        if (!string.IsNullOrWhiteSpace(dispatch?.FlowTaskId))
        {
            flowTask = await repository.GetByIdAsync(dispatch.FlowTaskId, ct);
        }

        flowTask ??= await repository.GetActiveByCraneTaskNoAsync(@event.TaskNo, ct);

        // Step 2A: Automatic inbound - complete WMS and resume the flow only after crane completion.
        if (flowTask != null)
        {
            var handled = await CompleteAutomaticInboundAsync(wmsService, flowTask, @event, ct);
            if (handled && dispatch is not null)
            {
                await dispatchService.MarkCompletedAsync(@event.TaskNo, ct);
            }
            return;
        }

        // Step 2B: Manual inbound - complete WMS from the task tracker because there is no FlowTask.
        if (_manualCraneTaskTracker.TryGet(@event.TaskNo, out var manualTask))
        {
            if (manualTask.CallWmsComplete)
            {
                await wmsService.CompleteTransferInboundAsync(manualTask.PositionCode, ct);
                _logger.LogInformation(
                    "Completed manual WMS inbound transfer from crane completion. CraneTaskNo={TaskNo}, Position={Position}",
                    @event.TaskNo,
                    manualTask.PositionCode);
            }
            else
            {
                _logger.LogInformation(
                    "Skip manual WMS inbound complete because CallWmsComplete=false. CraneTaskNo={TaskNo}, Position={Position}",
                    @event.TaskNo,
                    manualTask.PositionCode);
            }

            _manualCraneTaskTracker.Remove(@event.TaskNo);
            if (dispatch is not null)
            {
                await dispatchService.MarkCompletedAsync(@event.TaskNo, ct);
            }
            return;
        }

        if (dispatch is not null && string.IsNullOrWhiteSpace(dispatch.FlowTaskId))
        {
            await dispatchService.MarkCompletedAsync(@event.TaskNo, ct);
            _logger.LogInformation(
                "Completed manual crane dispatch from PLC completion. TaskNo={TaskNo}, TaskType={TaskType}",
                @event.TaskNo,
                @event.TaskType);
            return;
        }

        _logger.LogWarning(
            "Cannot resolve context for completed crane task. TaskNo={TaskNo}, TaskType={TaskType}, CompleteStatus={CompleteStatus}, ConfirmTaskNo={ConfirmTaskNo}",
            @event.TaskNo,
            @event.TaskType,
            @event.CompleteStatus,
            @event.ConfirmData?.TaskNo);
    }

    [CapSubscribe("Wcs.Okamura.Events.CraneErrorEvent")]
    public async Task HandleCraneErrorAsync(CraneErrorEvent @event, CancellationToken ct)
    {
        if (@event.SpecialError == null
            || @event.ErrorSource is not ("SpecialErrorRaised" or "SpecialError"))
        {
            return;
        }

        // Step 1: Resolve services and determine whether this is automatic or manual inbound.
        using var scope = _serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFlowTaskRepository>();
        var craneService = scope.ServiceProvider.GetRequiredService<ICraneService>();
        var wmsService = scope.ServiceProvider.GetRequiredService<IWmsService>();
        var dispatchService = scope.ServiceProvider.GetRequiredService<CraneTaskDispatchService>();

        var (dispatch, flowTask) = await ResolveCraneSpecialErrorContextAsync(
            @event,
            repository,
            dispatchService,
            ct);

        if (@event.ErrorSource == "SpecialErrorRaised")
        {
            await PublishWarehouseSpecialErrorAsync(@event, flowTask, ct);
            return;
        }

        // Step 2: Route by D3054 special error.
        if (@event.SpecialError == CraneSpecialErrorType.NoItemAtPick)
        {
            await HandleNoItemAtPickAsync(wmsService, dispatchService, flowTask, @event, ct);
            return;
        }

        if (@event.SpecialError != CraneSpecialErrorType.PutPositionOccupied)
        {
            _logger.LogWarning(
                "D3054 special error has no recovery flow. SpecialError={SpecialError}, TaskNo={TaskNo}, TaskType={TaskType}",
                @event.SpecialError,
                @event.TaskNo,
                @event.TaskType);
            return;
        }

        if (@event.TaskType is not (CraneTaskType.Inbound or CraneTaskType.InternalMove))
        {
            _logger.LogWarning(
                "D3054=2 PutPositionOccupied is ignored for non-inbound crane task. TaskNo={TaskNo}, TaskType={TaskType}",
                @event.TaskNo,
                @event.TaskType);
            return;
        }

        if (flowTask != null)
        {
            await HandleAutomaticPutOccupiedAsync(repository, craneService, wmsService, dispatchService, flowTask, @event, ct);
            return;
        }

        if (_manualCraneTaskTracker.TryGet(@event.TaskNo, out var manualTask))
        {
            await HandleManualPutOccupiedAsync(craneService, wmsService, dispatchService, @event, manualTask, ct);
            return;
        }

        _logger.LogWarning(
            "D3054=2 PutPositionOccupied has no recovery context. TaskNo={TaskNo}, TaskType={TaskType}",
            @event.TaskNo,
            @event.TaskType);
    }

    private async Task<(CraneTaskDispatchDbModel? Dispatch, FlowTask? FlowTask)> ResolveCraneSpecialErrorContextAsync(
        CraneErrorEvent @event,
        IFlowTaskRepository repository,
        CraneTaskDispatchService dispatchService,
        CancellationToken ct)
    {
        var taskNo = @event.TaskNo;
        if (taskNo == 0
            && @event.ConfirmData is { IsCleared: false } confirm
            && confirm.TaskNo > 0)
        {
            taskNo = confirm.TaskNo;
        }

        CraneTaskDispatchDbModel? dispatch = null;
        FlowTask? flowTask = null;

        if (taskNo > 0)
        {
            dispatch = await dispatchService.GetByTaskNoAsync(taskNo, ct);
            if (!string.IsNullOrWhiteSpace(dispatch?.FlowTaskId))
            {
                flowTask = await repository.GetByIdAsync(dispatch.FlowTaskId, ct);
            }

            flowTask ??= await repository.GetActiveByCraneTaskNoAsync(taskNo, ct);
        }

        if (flowTask == null && @event.ErrorSource == "SpecialError")
        {
            var candidate = await repository.GetSingleActiveAfterDropWithCraneAsync(ct);
            if (candidate?.CraneTaskNo is int craneTaskNo)
            {
                taskNo = craneTaskNo;
                flowTask = candidate;
                dispatch = await dispatchService.GetByTaskNoAsync(taskNo, ct);
                _logger.LogWarning(
                    "Resolved crane special-error recovery context from DB fallback. TaskNo={TaskNo}, FlowTaskId={FlowTaskId}",
                    taskNo,
                    flowTask.Id);
            }
        }

        if (taskNo > 0 && @event.TaskNo != taskNo)
        {
            @event.TaskNo = taskNo;
        }

        if (@event.TaskType is null or CraneTaskType.None)
        {
            if (dispatch != null)
            {
                @event.TaskType = (CraneTaskType)dispatch.TaskType;
            }
            else if (@event.ConfirmData is { IsCleared: false } confirmData
                && confirmData.TaskType != CraneTaskType.None)
            {
                @event.TaskType = confirmData.TaskType;
            }
        }

        return (dispatch, flowTask);
    }

    private async Task PublishWarehouseSpecialErrorAsync(
        CraneErrorEvent @event,
        FlowTask? flowTask,
        CancellationToken ct)
    {
        var d3054 = (int)@event.SpecialError!.Value;
        var taskId = flowTask?.Id.ToString() ?? string.Empty;
        var code = $"D3054_{d3054}";
        var message = $"Crane special error D3054={d3054} ({@event.SpecialError})";

        await _publisher.PublishAsync(new BizError(taskId, "CRANE0004", message), ct);

        _logger.LogWarning(
            "Published warehouse special error. TaskId={TaskId}, Code={Code}, TaskNo={TaskNo}, TaskType={TaskType}",
            taskId,
            code,
            @event.TaskNo,
            @event.TaskType);
    }

    private async Task HandleNoItemAtPickAsync(
        IWmsService wmsService,
        CraneTaskDispatchService dispatchService,
        FlowTask? flowTask,
        CraneErrorEvent @event,
        CancellationToken ct)
    {
        const int errorPositionStatus = 3;

        var pickPositionCode = ResolvePickPositionCode(@event, flowTask);
        if (pickPositionCode is not null)
        {
            await wmsService.UpdatePositionStatusAsync(pickPositionCode, errorPositionStatus, ct);
            _logger.LogWarning(
                "D3054=1 NoItemAtPick. Marked warehouse pick position as error. Position={Position}, TaskNo={TaskNo}, TaskType={TaskType}",
                pickPositionCode,
                @event.TaskNo,
                @event.TaskType);
        }
        else
        {
            _logger.LogWarning(
                "D3054=1 NoItemAtPick. No warehouse pick position to update in WMS. TaskNo={TaskNo}, TaskType={TaskType}",
                @event.TaskNo,
                @event.TaskType);
        }

        await dispatchService.MarkFailedAsync(
            @event.TaskNo,
            "D3054=1 NoItemAtPick",
            errorCode: (int)CraneSpecialErrorType.NoItemAtPick,
            ct);

        if (flowTask is not null && flowTask.Status == FlowStatus.Active)
        {
            await _publisher.PublishAsync(
                new FlowErrorOccurred(
                    flowTask.Id.ToString(),
                    flowTask.CurrentStep,
                    "CRANE0005: D3054=1 NoItemAtPick"),
                ct);

            _logger.LogWarning(
                "D3054=1 NoItemAtPick. Cancelled FlowTask. TaskId={TaskId}, Step={Step}, CraneTaskNo={TaskNo}",
                flowTask.Id,
                flowTask.CurrentStep,
                @event.TaskNo);
            return;
        }

        if (_manualCraneTaskTracker.TryGet(@event.TaskNo, out _))
        {
            _manualCraneTaskTracker.Remove(@event.TaskNo);
            _logger.LogWarning(
                "D3054=1 NoItemAtPick. Removed manual crane task tracker entry. TaskNo={TaskNo}",
                @event.TaskNo);
        }
    }

    private static string? ResolvePickPositionCode(CraneErrorEvent @event, FlowTask? flowTask)
    {
        if (@event.ConfirmData is { IsCleared: false } confirm)
        {
            var pickFromConfirm = new CommonCranePosition(
                confirm.StartPosX,
                confirm.StartPosZ,
                confirm.StartPosY);

            if (!IsStationPosition(pickFromConfirm))
            {
                return ToPositionCode(pickFromConfirm);
            }
        }

        if (flowTask?.CranePositions is not { Count: > 0 } positions)
        {
            return null;
        }

        if (@event.TaskType is CraneTaskType.Outbound or CraneTaskType.InternalMove)
        {
            var pick = positions[0];
            return IsStationPosition(pick) ? null : ToPositionCode(pick);
        }

        return null;
    }

    private static bool IsStationPosition(CommonCranePosition position)
        => (position.Row, position.Floor, position.Position) == (
            CraneStationPositions.Pick.Row,
            CraneStationPositions.Pick.Floor,
            CraneStationPositions.Pick.Position)
        || (position.Row, position.Floor, position.Position) == (
            CraneStationPositions.Put.Row,
            CraneStationPositions.Put.Floor,
            CraneStationPositions.Put.Position);

    private async Task<bool> CompleteAutomaticInboundAsync(
        IWmsService wmsService,
        FlowTask flowTask,
        CraneTaskCompletedEvent @event,
        CancellationToken ct)
    {
        // Step 2A.1: Only inbound-side crane tasks in AfterDrop represent a warehouse put completion.
        if (@event.TaskType is not (CraneTaskType.Inbound or CraneTaskType.InternalMove)
            || flowTask.CurrentStep != FlowStep.AfterDrop
            || !flowTask.ToStation.HasWarehouseDoor)
        {
            _logger.LogInformation(
                "No WMS complete action configured for crane task type {TaskType}. TaskNo={TaskNo}",
                @event.TaskType,
                @event.TaskNo);
            return true;
        }

        // Step 2A.2: Complete WMS for the current target position.
        var putPosition = flowTask.CranePositions?.FirstOrDefault();
        if (putPosition == null)
        {
            _logger.LogWarning("Cannot complete inbound WMS transfer; FlowTask {TaskId} has no crane position", flowTask.Id);
            return false;
        }

        var positionCode = ToPositionCode(putPosition);
        await wmsService.CompleteTransferInboundAsync(positionCode, ct);
        _logger.LogInformation(
            "Completed WMS inbound transfer from crane completion. TaskId={TaskId}, CraneTaskNo={TaskNo}, TaskType={TaskType}, Position={Position}",
            flowTask.Id,
            @event.TaskNo,
            @event.TaskType,
            positionCode);

        // Step 2A.3: Resume the robot flow after WMS is consistent.
        await _publisher.PublishAsync(new FlowResumed(flowTask.Id), ct);
        return true;
    }

    private async Task HandleAutomaticPutOccupiedAsync(
        IFlowTaskRepository repository,
        ICraneService craneService,
        IWmsService wmsService,
        CraneTaskDispatchService dispatchService,
        FlowTask flowTask,
        CraneErrorEvent @event,
        CancellationToken ct)
    {
        // Step 2A.1: Automatic flow already has candidate positions from WMS in FlowTask.CranePositions.
        var positions = flowTask.CranePositions;
        if (positions == null || positions.Count < 2)
        {
            _logger.LogWarning(
                "Cannot create automatic internal move for FlowTask {TaskId}; expected at least 2 crane positions, got {Count}",
                flowTask.Id,
                positions?.Count ?? 0);
            return;
        }

        var failedPosition = positions[0];
        var nextPosition = positions[1];
        var failedPositionCode = ToPositionCode(failedPosition);
        var nextPositionCode = ToPositionCode(nextPosition);

        // Step 2A.2: Tell WMS that the failed location is error and the replacement is now locked.
        await wmsService.HandleErrorInboundAsync(failedPositionCode, nextPositionCode, ct);

        // Step 2A.3: Send recovery internal move after WMS accepted the replacement.
        var retryTaskNo = await _craneTaskNoGenerator.NextAsync(ct);
        var idempotencyKey = $"flow:{flowTask.Id}:crane:internal-move:put-occupied:{@event.TaskNo}";
        var reservation = await dispatchService.ReserveFlowTaskDispatchAsync(
            flowTask.Id,
            retryTaskNo,
            CraneTaskType.InternalMove,
            idempotencyKey,
            ct);

        retryTaskNo = reservation.Dispatch.TaskNo;
        var originalPositions = positions.ToList();
        flowTask.CraneTaskNo = retryTaskNo;
        flowTask.CranePositions = positions.Skip(1).ToList();
        await repository.UpdateAsync(flowTask, ct);

        if (!reservation.Created)
        {
            _logger.LogInformation(
                "Use existing internal move crane dispatch. FlowTask={TaskId}, TaskNo={TaskNo}, Status={Status}",
                flowTask.Id,
                reservation.Dispatch.TaskNo,
                reservation.Dispatch.Status);
            return;
        }

        _logger.LogWarning(
            "D3054=2 recovered with D3057=1. Create internal move crane task. FlowTask={TaskId}, Pick={Pick}, Put={Put}, NewTaskNo={TaskNo}",
            flowTask.Id,
            failedPositionCode,
            nextPositionCode,
            retryTaskNo);

        var result = await craneService.SendInternalMoveAsync(new InternalMoveTask(
            retryTaskNo,
            ToOkamuraPosition(failedPosition),
            ToOkamuraPosition(nextPosition)), ct);

        if (!result.Success)
        {
            await dispatchService.MarkFailedAsync(
                reservation.Dispatch.TaskNo,
                result.ErrorMessage ?? "Crane internal move dispatch failed",
                result.ErrorCode,
                ct);
            flowTask.CraneTaskNo = null;
            flowTask.CranePositions = originalPositions;
            await repository.UpdateAsync(flowTask, ct);
            _logger.LogWarning(
                "Internal move task failed. FlowTask={TaskId}, TaskNo={TaskNo}, ErrorCode={ErrorCode}, Error={Error}",
                flowTask.Id,
                retryTaskNo,
                result.ErrorCode,
                result.ErrorMessage);
            return;
        }

        await dispatchService.MarkAcceptedAsync(reservation.Dispatch.TaskNo, ct);
    }

    private async Task HandleManualPutOccupiedAsync(
        ICraneService craneService,
        IWmsService wmsService,
        CraneTaskDispatchService dispatchService,
        CraneErrorEvent @event,
        ManualCraneTaskState manualTask,
        CancellationToken ct)
    {
        // Step 2B.1: Manual inbound has no FlowTask, so use the tracked failed put position.
        var failedPositionCode = manualTask.PositionCode;

        // Step 2B.2: Ask WMS for any empty inbound position; item metadata belongs to confirm, not this lookup.
        var replacementPositionCode = await wmsService.FindReplacementInboundPositionAsync(
            failedPositionCode,
            ct);
        await wmsService.HandleErrorInboundAsync(failedPositionCode, replacementPositionCode, ct);

        // Step 2B.3: Send recovery internal move for manual inbound.
        var retryTaskNo = await _craneTaskNoGenerator.NextAsync(ct);
        await dispatchService.ReserveManualDispatchAsync(
            retryTaskNo,
            CraneTaskType.InternalMove,
            $"Manual recovery internal move from {@event.TaskNo}: {failedPositionCode} -> {replacementPositionCode}",
            ct);

        var result = await craneService.SendInternalMoveAsync(new InternalMoveTask(
            retryTaskNo,
            ParsePositionCode(failedPositionCode),
            ParsePositionCode(replacementPositionCode)));

        if (!result.Success)
        {
            await dispatchService.MarkFailedAsync(
                retryTaskNo,
                result.ErrorMessage ?? "Manual recovery internal move failed",
                result.ErrorCode,
                ct);
            _logger.LogWarning(
                "Manual internal move task failed. OriginalTaskNo={OriginalTaskNo}, RetryTaskNo={RetryTaskNo}, ErrorCode={ErrorCode}, Error={Error}",
                @event.TaskNo,
                retryTaskNo,
                result.ErrorCode,
                result.ErrorMessage);
            return;
        }

        await dispatchService.MarkAcceptedAsync(retryTaskNo, ct);

        // Step 2B.4: Track replacement task so its completion event can call WMS complete.
        _manualCraneTaskTracker.Remove(@event.TaskNo);
        _manualCraneTaskTracker.TrackInbound(
            retryTaskNo,
            replacementPositionCode,
            manualTask.CallWmsComplete,
            manualTask.StageCode,
            manualTask.CassetteId,
            manualTask.Size,
            manualTask.Quantity);

        _logger.LogInformation(
            "Tracked manual inbound recovery task. OriginalTaskNo={OriginalTaskNo}, RetryTaskNo={RetryTaskNo}, FailedPosition={FailedPosition}, ReplacementPosition={ReplacementPosition}, CallWmsComplete={CallWmsComplete}",
            @event.TaskNo,
            retryTaskNo,
            failedPositionCode,
            replacementPositionCode,
            manualTask.CallWmsComplete);
    }

    private static OkamuraCranePosition ToOkamuraPosition(CommonCranePosition position)
        => new(position.Row, position.Floor, position.Position);

    private static string ToPositionCode(CommonCranePosition position)
        => $"F{position.Floor}-R{position.Row}-P{position.Position}";

    private static OkamuraCranePosition ParsePositionCode(string positionCode)
    {
        var parts = positionCode.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
        {
            throw new FormatException($"Invalid position code: {positionCode}");
        }

        var floor = int.Parse(parts[0].TrimStart('F', 'f'));
        var row = int.Parse(parts[1].TrimStart('R', 'r'));
        var position = int.Parse(parts[2].TrimStart('P', 'p'));
        return new OkamuraCranePosition(row, floor, position);
    }
}
