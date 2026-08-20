using Microsoft.EntityFrameworkCore;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Models;
using Wcs.Okamura.Enums;

namespace Wcs.Api.Services;

public sealed record CraneTaskDispatchReservation(CraneTaskDispatchDbModel Dispatch, bool Created);

public sealed class CraneTaskDispatchService(
    WcsDbContext context,
    ILogger<CraneTaskDispatchService> logger)
{
    private readonly WcsDbContext _context = context;
    private readonly ILogger<CraneTaskDispatchService> _logger = logger;

    private static readonly CraneTaskDispatchStatus[] NonRetryableStatuses =
    [
        CraneTaskDispatchStatus.Dispatching,
        CraneTaskDispatchStatus.Accepted,
        CraneTaskDispatchStatus.Completing,
        CraneTaskDispatchStatus.Completed
    ];

    public Task<CraneTaskDispatchReservation> ReserveFlowTaskDispatchAsync(
        string flowTaskId,
        int taskNo,
        CraneTaskType taskType,
        string idempotencyKey,
        CancellationToken ct = default)
        => ReserveDispatchAsync(
            taskNo,
            taskType,
            idempotencyKey,
            flowTaskId,
            "Reserved before sending crane instruction",
            ct);

    public Task<CraneTaskDispatchReservation> ReserveManualDispatchAsync(
        int taskNo,
        CraneTaskType taskType,
        string? lastMessage = null,
        CancellationToken ct = default)
        => ReserveDispatchAsync(
            taskNo,
            taskType,
            $"manual:crane:{(int)taskType}:{taskNo}",
            flowTaskId: null,
            lastMessage ?? "Manual crane command reserved",
            ct);

    private async Task<CraneTaskDispatchReservation> ReserveDispatchAsync(
        int taskNo,
        CraneTaskType taskType,
        string idempotencyKey,
        string? flowTaskId,
        string lastMessage,
        CancellationToken ct)
    {
        var existingByKey = await _context.CraneTaskDispatches
            .Where(d => d.IdempotencyKey == idempotencyKey)
            .Where(d => NonRetryableStatuses.Contains(d.Status))
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (existingByKey is not null)
        {
            return new CraneTaskDispatchReservation(existingByKey, Created: false);
        }

        var activeTaskNo = await _context.CraneTaskDispatches
            .Where(d => d.TaskNo == taskNo)
            .Where(d => d.Status == CraneTaskDispatchStatus.Dispatching
                || d.Status == CraneTaskDispatchStatus.Accepted
                || d.Status == CraneTaskDispatchStatus.Completing)
            .FirstOrDefaultAsync(ct);

        if (activeTaskNo is not null)
        {
            throw new InvalidOperationException(
                $"Crane task number {taskNo} is already active for dispatch {activeTaskNo.Id}.");
        }

        var dispatch = new CraneTaskDispatchDbModel
        {
            TaskNo = taskNo,
            FlowTaskId = flowTaskId,
            TaskType = (int)taskType,
            Status = CraneTaskDispatchStatus.Dispatching,
            IdempotencyKey = idempotencyKey,
            LastMessage = lastMessage
        };

        _context.CraneTaskDispatches.Add(dispatch);
        await _context.SaveChangesAsync(ct);
        return new CraneTaskDispatchReservation(dispatch, Created: true);
    }

    public async Task<CraneTaskDispatchDbModel?> GetByTaskNoAsync(int taskNo, CancellationToken ct = default)
        => await _context.CraneTaskDispatches
            .Where(d => d.TaskNo == taskNo)
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task MarkAcceptedAsync(int taskNo, CancellationToken ct = default)
    {
        var dispatch = await GetByTaskNoAsync(taskNo, ct);
        if (dispatch is null)
        {
            _logger.LogWarning("Cannot mark crane task accepted; dispatch not found. TaskNo={TaskNo}", taskNo);
            return;
        }

        if (dispatch.Status != CraneTaskDispatchStatus.Dispatching)
        {
            return;
        }

        dispatch.Status = CraneTaskDispatchStatus.Accepted;
        dispatch.AcceptedAt = DateTime.UtcNow;
        dispatch.LastMessage = "Crane instruction accepted by PLC";
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> TryBeginCompletionAsync(
        int taskNo,
        TaskCompleteStatus completeStatus,
        int errorCode,
        string? errorNote,
        CancellationToken ct = default)
    {
        var dispatch = await GetByTaskNoAsync(taskNo, ct);
        if (dispatch is null)
        {
            return true;
        }

        if (dispatch.Status is CraneTaskDispatchStatus.Completing or CraneTaskDispatchStatus.Completed)
        {
            _logger.LogInformation(
                "Skip duplicate crane completion. TaskNo={TaskNo}, Status={Status}",
                taskNo,
                dispatch.Status);
            return false;
        }

        dispatch.Status = CraneTaskDispatchStatus.Completing;
        dispatch.CompleteStatus = (int)completeStatus;
        dispatch.ErrorCode = errorCode;
        dispatch.ErrorNote = errorNote;
        dispatch.CompletingAt = DateTime.UtcNow;
        dispatch.LastMessage = "Crane completion event received";
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task MarkCompletedAsync(int taskNo, CancellationToken ct = default)
    {
        var dispatch = await GetByTaskNoAsync(taskNo, ct);
        if (dispatch is null)
        {
            _logger.LogWarning("Cannot mark crane task completed; dispatch not found. TaskNo={TaskNo}", taskNo);
            return;
        }

        dispatch.Status = CraneTaskDispatchStatus.Completed;
        dispatch.CompletedAt = DateTime.UtcNow;
        dispatch.LastMessage = "Crane task completed and handled by WCS";
        await _context.SaveChangesAsync(ct);
    }

    public async Task MarkFailedAsync(
        int taskNo,
        string message,
        int? errorCode = null,
        CancellationToken ct = default)
    {
        var dispatch = await GetByTaskNoAsync(taskNo, ct);
        if (dispatch is null)
        {
            _logger.LogWarning("Cannot mark crane task failed; dispatch not found. TaskNo={TaskNo}", taskNo);
            return;
        }

        dispatch.Status = CraneTaskDispatchStatus.Failed;
        dispatch.ErrorCode = errorCode;
        dispatch.ErrorNote = message;
        dispatch.FailedAt = DateTime.UtcNow;
        dispatch.LastMessage = message;
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<CraneTaskDispatchDbModel>> ListRecentAsync(
        int limit = 100,
        CraneTaskDispatchStatus? status = null,
        CraneTaskType? taskType = null,
        bool? manualOnly = null,
        bool activeOnly = false,
        CancellationToken ct = default)
    {
        limit = Math.Clamp(limit, 1, 500);

        var query = _context.CraneTaskDispatches.AsNoTracking().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }
        else if (activeOnly)
        {
            query = query.Where(d =>
                d.Status == CraneTaskDispatchStatus.Dispatching
                || d.Status == CraneTaskDispatchStatus.Accepted
                || d.Status == CraneTaskDispatchStatus.Completing);
        }

        if (taskType.HasValue)
        {
            query = query.Where(d => d.TaskType == (int)taskType.Value);
        }

        if (manualOnly == true)
        {
            query = query.Where(d => d.FlowTaskId == null || d.FlowTaskId == "");
        }
        else if (manualOnly == false)
        {
            query = query.Where(d => d.FlowTaskId != null && d.FlowTaskId != "");
        }

        return await query
            .OrderByDescending(d => d.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<int?> TryGetActiveInboundTaskNoAsync(CancellationToken ct = default)
    {
        var dispatch = await _context.CraneTaskDispatches
            .AsNoTracking()
            .Where(d => d.TaskType == (int)CraneTaskType.Inbound)
            .Where(d =>
                d.Status == CraneTaskDispatchStatus.Dispatching
                || d.Status == CraneTaskDispatchStatus.Accepted
                || d.Status == CraneTaskDispatchStatus.Completing)
            .OrderByDescending(d => d.AcceptedAt ?? d.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return dispatch?.TaskNo;
    }

    public async Task RecordD2451RaisedAsync(int taskNo, CancellationToken ct = default)
    {
        const int d2451Value = 1;
        var dispatch = await GetByTaskNoAsync(taskNo, ct);
        if (dispatch is null)
        {
            _logger.LogWarning(
                "Cannot record D2451 raised; dispatch not found. TaskNo={TaskNo}",
                taskNo);
            return;
        }

        dispatch.QrFinishedRaisedValue = d2451Value;
        dispatch.QrFinishedRaisedAt = DateTime.UtcNow;
        dispatch.QrFinishedHadCachedQr = true;
        dispatch.QrFinishedClearedAt = null;
        dispatch.UpdatedAt = DateTime.UtcNow;
        dispatch.LastMessage = "D2451 raised=1 after D2030 1→0";
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Recorded D2451 raised. TaskNo={TaskNo}, Value={Value}, DispatchId={DispatchId}",
            taskNo,
            d2451Value,
            dispatch.Id);
    }

    public async Task RecordD2451ClearedAsync(int taskNo, CancellationToken ct = default)
    {
        var dispatch = await GetByTaskNoAsync(taskNo, ct);
        if (dispatch is null)
        {
            _logger.LogWarning("Cannot record D2451 cleared; dispatch not found. TaskNo={TaskNo}", taskNo);
            return;
        }

        dispatch.QrFinishedClearedAt = DateTime.UtcNow;
        dispatch.UpdatedAt = DateTime.UtcNow;
        dispatch.LastMessage = dispatch.QrFinishedRaisedValue is int raised
            ? $"D2451 cleared after raised={raised}"
            : "D2451 cleared";
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Recorded D2451 cleared. TaskNo={TaskNo}, RaisedValue={RaisedValue}, DispatchId={DispatchId}",
            taskNo,
            dispatch.QrFinishedRaisedValue,
            dispatch.Id);
    }
}
