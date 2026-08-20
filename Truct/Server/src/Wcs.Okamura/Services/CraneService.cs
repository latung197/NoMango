namespace Wcs.Okamura.Services;

using Microsoft.Extensions.Logging;
using Wcs.OpcUa.Contracts;
using Wcs.Okamura.Models;
using Wcs.Okamura.Enums;

public class CraneService(ILogger<CraneService> logger, IOpcUaClient opcUaClient) : ICraneService
{
    private readonly ILogger<CraneService> _logger = logger;
    private readonly IOpcUaClient _opc = opcUaClient;
    private readonly SemaphoreSlim _dispatchLock = new(1, 1);
    private static readonly TimeSpan DefaultReadyWaitTimeout = TimeSpan.FromSeconds(120);
    private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromMilliseconds(200);

    // =========================================================
    // PUBLIC API
    // =========================================================

    public Task<CraneTaskResult> SendInboundAsync(InboundTask task, CancellationToken ct = default, bool waitForReady = true)
        => SendTaskAsync(task, CraneTaskType.Inbound, ct, waitForReady);

    public Task<CraneTaskResult> SendOutboundAsync(OutboundTask task, CancellationToken ct = default, bool waitForReady = true)
        => SendTaskAsync(task, CraneTaskType.Outbound, ct, waitForReady);

    public Task<CraneTaskResult> SendInternalMoveAsync(InternalMoveTask task, CancellationToken ct = default, bool waitForReady = true)
        => SendTaskAsync(task, CraneTaskType.InternalMove, ct, waitForReady);

    public Task<CraneTaskResult> SendStationMoveAsync(StationMoveTask task, CancellationToken ct = default, bool waitForReady = true)
        => SendTaskAsync(task, CraneTaskType.StationMove, ct, waitForReady);

    public Task<CraneReadyStatus> GetReadyStatusAsync(CancellationToken ct = default)
        => ReadReadyStatusAsync(ct);

    public async Task<bool> WaitUntilReadyAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);

        try
        {
            while (true)
            {
                var status = await ReadReadyStatusAsync(timeoutCts.Token);
                if (status.IsReady)
                    return true;

                await Task.Delay(ReadyPollInterval, timeoutCts.Token);
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return false;
        }
    }

    public async Task<bool> CancelCurrentTaskAsync(int taskNo)
    {
        await _opc.WriteValueAsync(OpcTags.D3016, taskNo);
        await WriteTaskFinishAffirmAsync(3);
        return true;
    }

    public async Task ClearTaskDataAsync()
    {
        await _opc.WriteValueAsync(OpcTags.D3010, 0);
        await _opc.WriteValueAsync(OpcTags.D3003, 0);
        await _opc.WriteValueAsync(OpcTags.D3004, 0);
        await _opc.WriteValueAsync(OpcTags.D3005, 0);
        await _opc.WriteValueAsync(OpcTags.D3006, 0);
        await _opc.WriteValueAsync(OpcTags.D3007, 0);
        await _opc.WriteValueAsync(OpcTags.D3008, 0);
        await _opc.WriteValueAsync(OpcTags.D3009, 0);
        await _opc.WriteValueAsync(OpcTags.D3016, 0);
    }

    public Task WriteTaskFinishAffirmAsync(int value)
        => _opc.WriteValueAsync(OpcTags.D3013, value);

    public Task WriteTaskErrorAffirmAsync(int value)
        => _opc.WriteValueAsync(OpcTags.D3014, value);

    public async Task<CraneConfirmData> ReadConfirmDataAsync()
    {
        var tunnel = await _opc.ReadValueAsync(OpcTags.D3042);
        var startX = await _opc.ReadValueAsync(OpcTags.D3043);
        var startY = await _opc.ReadValueAsync(OpcTags.D3044);
        var startZ = await _opc.ReadValueAsync(OpcTags.D3045);
        var endX = await _opc.ReadValueAsync(OpcTags.D3046);
        var endY = await _opc.ReadValueAsync(OpcTags.D3047);
        var endZ = await _opc.ReadValueAsync(OpcTags.D3048);
        var taskType = await _opc.ReadValueAsync(OpcTags.D3049);
        var taskNo = await _opc.ReadValueAsync(OpcTags.D3056);

        return new CraneConfirmData(
            Tunnel: Convert.ToInt32(tunnel.Value),
            StartPosX: Convert.ToInt32(startX.Value),
            StartPosY: Convert.ToInt32(startY.Value),
            StartPosZ: Convert.ToInt32(startZ.Value),
            EndPosX: Convert.ToInt32(endX.Value),
            EndPosY: Convert.ToInt32(endY.Value),
            EndPosZ: Convert.ToInt32(endZ.Value),
            TaskType: (CraneTaskType)Convert.ToInt32(taskType.Value),
            TaskNo: Convert.ToInt32(taskNo.Value)
        );
    }

    public async Task HandleTaskFinishStateChangedAsync(TaskCompleteStatus status)
    {
        if (status != TaskCompleteStatus.None)
        {
            await ClearTaskDataAsync();
        }
        await WriteTaskFinishAffirmAsync((int)status);
    }

    public async Task<CraneFeedback> GetCurrentFeedbackAsync()
    {
        var taskNo = await _opc.ReadValueAsync(OpcTags.D3056);
        var taskType = await _opc.ReadValueAsync(OpcTags.D3049);
        var executeFeedback = await _opc.ReadValueAsync(OpcTags.D3050);
        var completeStatus = await _opc.ReadValueAsync(OpcTags.D3053);
        var specialError = await _opc.ReadValueAsync(OpcTags.D3054);
        var status = await _opc.ReadValueAsync(OpcTags.D3059);
        var errorCode = await _opc.ReadValueAsync(OpcTags.D3051);
        var errorNote = await _opc.ReadValueAsync(OpcTags.D3052);
        return new CraneFeedback(
            TaskNo: Convert.ToInt32(taskNo.Value),
            TaskType: (CraneTaskType)Convert.ToInt32(taskType.Value),
            ExecuteFeedback: (TaskExecuteFeedback)Convert.ToInt32(executeFeedback.Value),
            CompleteStatus: (TaskCompleteStatus)Convert.ToInt32(completeStatus.Value),
            SpecialError: (CraneSpecialErrorType)Convert.ToInt32(specialError.Value),
            Status: (CraneStatus)Convert.ToInt32(status.Value),
            Position: await GetCranePositionAsync(),
            ErrorCode: Convert.ToInt32(errorCode.Value),
            ErrorNote: Convert.ToString(errorNote.Value) ?? string.Empty
        );
    }

    public async Task<CranePosition> GetCranePositionAsync()
    {
        var column = await _opc.ReadValueAsync(OpcTags.D3062);
        var level = await _opc.ReadValueAsync(OpcTags.D3063);
        var forkPosition = await _opc.ReadValueAsync(OpcTags.D3068);
        return new CranePosition(
            Row: Convert.ToInt32(column.Value),
            Floor: Convert.ToInt32(level.Value),
            Position: Convert.ToInt32(forkPosition.Value)
        );
    }

    // =========================================================
    // CORE LOGIC
    // =========================================================

    private async Task<CraneTaskResult> SendTaskAsync(
        CraneTask task,
        CraneTaskType taskType,
        CancellationToken ct,
        bool waitForReady = true)
    {
        await _dispatchLock.WaitAsync(ct);
        try
        {
            if (waitForReady)
            {
                _logger.LogInformation(
                    "[Crane] TaskNo={TaskNo} Type={TaskType} — chờ crane sẵn sàng (timeout {Timeout}s)",
                    task.TaskNo, taskType, DefaultReadyWaitTimeout.TotalSeconds);

                if (!await WaitUntilReadyAsync(DefaultReadyWaitTimeout, ct))
                {
                    var status = await ReadReadyStatusAsync(ct);
                    return Fail(
                        status.Reason ?? $"Timeout chờ crane sẵn sàng ({DefaultReadyWaitTimeout.TotalSeconds}s)",
                        status.ErrorCode != 0 ? status.ErrorCode : 9002);
                }
            }
            else
            {
                _logger.LogInformation(
                    "[Crane] TaskNo={TaskNo} Type={TaskType} — kiểm tra crane sẵn sàng (không chờ)",
                    task.TaskNo, taskType);

                var status = await ReadReadyStatusAsync(ct);
                if (!status.IsReady)
                    return Fail(status.Reason ?? "Crane chưa sẵn sàng", status.ErrorCode != 0 ? status.ErrorCode : 9002);
            }

            return await SendTaskWhenReadyAsync(task, taskType, ct, waitForReady);
        }
        finally
        {
            _dispatchLock.Release();
        }
    }

    private async Task<CraneTaskResult> SendTaskWhenReadyAsync(
        CraneTask task,
        CraneTaskType taskType,
        CancellationToken ct,
        bool waitForReady = true)
    {
        if (taskType == CraneTaskType.Inbound && !await IsCvInboundCraneReadyAsync())
        {
            return Fail("D2021 (CV CraneReady inbound): yêu cầu=1, hiện tại=0", 9021);
        }

        if (taskType == CraneTaskType.Outbound && !await IsCvOutboundCraneReadyAsync())
        {
            return Fail("D2022 (CV CraneReady outbound): yêu cầu=1, hiện tại=0", 9023);
        }
        
        if (taskType == CraneTaskType.Outbound) {
            await _opc.WriteValueAsync(OpcTagsCvAgv.D2411, 0);
        }

        if (taskType == CraneTaskType.Inbound) {
            await _opc.WriteValueAsync(OpcTagsCvAgv.D2451, 0);
        }

        await WriteInstructionAsync(task, taskType);

        // Send exec signal
        await _opc.WriteValueAsync(OpcTags.D3010, 1);

        // Wait accept / instruction error (D3050)
        TaskExecuteFeedback execFb;
        if (waitForReady)
        {
            execFb = await WaitExecuteFeedbackAsync();
        }
        else
        {
            var fb = await _opc.ReadValueAsync(OpcTags.D3050);
            execFb = (TaskExecuteFeedback)Convert.ToInt32(fb.Value);
            if (execFb == 0)
                return Fail("D3050 (ExecuteFeedback): yêu cầu≠0 (Accepted/Error), hiện tại=0", 9050);
        }
        if (execFb == TaskExecuteFeedback.InstructionError)
        {
            var code = await SafeReadErrorCodeAsync();
            await ClearTaskDataAsync();
            return Fail("PLC instruction error", code);
        }

        if (execFb != TaskExecuteFeedback.Accepted)
        {
            return Fail($"Unexpected execute feedback: {(int)execFb}", await SafeReadErrorCodeAsync());
        }

        // D3050=1: verify confirm data khớp instruction rồi clear vùng chỉ thị
        if (!await VerifyConfirmDataAsync())
        {
            return Fail("PLC confirm data không khớp với instruction", 9022);
        }

        await _opc.WriteValueAsync(OpcTags.D3010, 0);

        // Task đã được PLC chấp nhận, return success
        // Việc lắng nghe kết quả từ crane sẽ được CraneMonitorService xử lý độc lập
        return new CraneTaskResult(
            Success: true,
            Status: TaskCompleteStatus.None, // Chưa có completion status
            SpecialError: CraneSpecialErrorType.None,
            ErrorCode: 9001,
            ErrorMessage: null
        );
    }

    private async Task<int> SafeReadErrorCodeAsync()
    {
        try { 
            var code = await _opc.ReadValueAsync(OpcTags.D3051);
            return Convert.ToInt32(code.Value); 
        }
        catch { return -1; }
    }

    // =========================================================
    // PRIVATE HELPERS
    // =========================================================

    private async Task<CraneReadyStatus> ReadReadyStatusAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        var executeFeedback = await _opc.ReadValueAsync(OpcTags.D3050);
        var completeStatus = await _opc.ReadValueAsync(OpcTags.D3053);
        var errorCode = await _opc.ReadValueAsync(OpcTags.D3051);
        var prepareCompleted = await _opc.ReadValueAsync(OpcTags.D3057);

        var d3050 = Convert.ToInt32(executeFeedback.Value);
        var d3053 = Convert.ToInt32(completeStatus.Value);
        var d3051 = Convert.ToInt32(errorCode.Value);
        var d3057 = Convert.ToInt32(prepareCompleted.Value);

        var issues = new List<string>();
        if (d3050 != 0)
            issues.Add($"D3050 (taskExe)={d3050}, yêu cầu 0");
        if (d3053 != 0)
            issues.Add($"D3053 (taskFinishState)={d3053}, yêu cầu 0");
        if (d3051 != 0)
            issues.Add($"D3051 (error)={d3051}, yêu cầu 0");
        if (d3057 != 1)
            issues.Add($"D3057 (dispatch)={d3057}, yêu cầu 1");

        return new CraneReadyStatus(
            IsReady: issues.Count == 0,
            TaskExecuteFeedback: d3050,
            TaskFinishState: d3053,
            ErrorCode: d3051,
            DispatchReady: d3057,
            Reason: issues.Count == 0 ? null : "Crane chưa sẵn sàng: " + string.Join("; ", issues));
    }

    /// <summary>D2021 – cassette đã ở vị trí station, sẵn sàng cho crane inbound lấy.</summary>
    private async Task<bool> IsCvInboundCraneReadyAsync()
    {
        var value = await _opc.ReadValueAsync(OpcTagsCvAgv.D2021);
        return Convert.ToInt32(value.Value) != 0;
    }

    /// <summary>D2022 – CV outbound sẵn sàng cho crane đặt hàng.</summary>
    private async Task<bool> IsCvOutboundCraneReadyAsync()
    {
        var value = await _opc.ReadValueAsync(OpcTagsCvAgv.D2022);
        return Convert.ToInt32(value.Value) != 0;
    }

    private async Task WriteInstructionAsync(CraneTask task, CraneTaskType type)
    {
        await _opc.WriteValueAsync(OpcTags.D3002, 1);
        await _opc.WriteValueAsync(OpcTags.D3009, (int)type);

        if (task is OutboundTask outboundTask)
        {
            await WriteStartPositionAsync(outboundTask.PickPosition);
            await WriteEndPositionAsync(outboundTask.PutPosition);
        }

        if (task is InboundTask inboundTask)
        {
            await WriteStartPositionAsync(inboundTask.PickPosition);
            await WriteEndPositionAsync(inboundTask.PutPosition);
        }

        if (task is InternalMoveTask internalMoveTask)
        {
            await WriteStartPositionAsync(internalMoveTask.PickPosition);
            await WriteEndPositionAsync(internalMoveTask.PutPosition);
        }

        await _opc.WriteValueAsync(OpcTags.D3016, task.TaskNo);
    }

    private async Task WriteStartPositionAsync(CranePosition position)
    {
        await _opc.WriteValueAsync(OpcTags.D3003, position.Row);
        await _opc.WriteValueAsync(OpcTags.D3004, position.Position);
        await _opc.WriteValueAsync(OpcTags.D3005, position.Floor);
    }

    private async Task WriteEndPositionAsync(CranePosition position)
    {
        await _opc.WriteValueAsync(OpcTags.D3006, position.Row);
        await _opc.WriteValueAsync(OpcTags.D3007, position.Position);
        await _opc.WriteValueAsync(OpcTags.D3008, position.Floor);
    }

    private async Task<TaskExecuteFeedback> WaitExecuteFeedbackAsync()
    {
        while (true)
        {
            var fb = await _opc.ReadValueAsync(OpcTags.D3050);
            if (Convert.ToInt32(fb.Value) != 0)
                return (TaskExecuteFeedback)Convert.ToInt32(fb.Value);

            await Task.Delay(100);
        }
    }

    /// <summary>
    /// So khớp confirm data (D3043–D3049, D3056) với instruction (D3003–D3009, D3016).
    /// </summary>
    private async Task<bool> VerifyConfirmDataAsync()
    {
        (string Instruction, string Confirm)[] pairs =
        [
            (OpcTags.D3003, OpcTags.D3043),
            (OpcTags.D3004, OpcTags.D3044),
            (OpcTags.D3005, OpcTags.D3045),
            (OpcTags.D3006, OpcTags.D3046),
            (OpcTags.D3007, OpcTags.D3047),
            (OpcTags.D3008, OpcTags.D3048),
            (OpcTags.D3009, OpcTags.D3049),
            (OpcTags.D3016, OpcTags.D3056),
        ];

        foreach (var (instruction, confirm) in pairs)
        {
            var sent = await _opc.ReadValueAsync(instruction);
            var ack = await _opc.ReadValueAsync(confirm);
            if (Convert.ToInt32(sent.Value) != Convert.ToInt32(ack.Value))
                return false;
        }

        return true;
    }

    private static CraneTaskResult Fail(string message, int errorCode)
    {
        return new CraneTaskResult(
            Success: false,
            Status: TaskCompleteStatus.None,
            SpecialError: CraneSpecialErrorType.None,
            ErrorCode: errorCode,
            ErrorMessage: message
        );
    }
}