namespace Wcs.Okamura.Services;

using Microsoft.Extensions.Logging;
using Opc.Ua;
using Wcs.OpcUa.Contracts;
using Wcs.Okamura.Enums;
using Wcs.Okamura.Events;
using Wcs.Okamura.Models;
using static Wcs.Okamura.OpcTagsCvAgv;

/// <summary>
/// Service xử lý monitoring crane qua OPC tags và publish events
/// </summary>
public class CraneMonitorService : ICraneMonitorService
{
    private readonly ILogger<CraneMonitorService> _logger;
    private readonly ICraneService _craneService;
    private readonly IOpcUaClient _opcUaClient;
    private readonly IWarehouseQrCodeStore _qrCodeStore;
    private readonly IStationHandshakeService _stationHandshakeService;
    private readonly Dictionary<string, DataValue> _lastValues = new();
    private readonly object _lockObject = new();
    private readonly SemaphoreSlim _heartbeatLock = new(1, 1);
    private int? _lastHeartbeatWritten;
    private const int CraneHeartbeatDelayMs = 1000;

    // Delegate để publish event - được inject từ bên ngoài
    private Func<object, Task>? _eventPublisher;

    /// <summary>Handshake D3054→D3014: đã ghi ACK, chờ TC clear D3054 và confirm data.</summary>
    private PendingSpecialErrorAck? _pendingSpecialErrorAck;

    private static readonly string[] ConfirmDataTags =
    [
        OpcTags.D3042, OpcTags.D3043, OpcTags.D3044, OpcTags.D3045,
        OpcTags.D3046, OpcTags.D3047, OpcTags.D3048, OpcTags.D3049,
        OpcTags.D3056
    ];

    private sealed class PendingSpecialErrorAck
    {
        public required CraneSpecialErrorType SpecialError { get; init; }
        public required int TaskNo { get; init; }
        public required CraneTaskType TaskType { get; init; }
        public required CraneConfirmData ConfirmData { get; init; }
        public int ErrorCode { get; init; }
        public string? ErrorNote { get; init; }
        public bool ErrorAffirmCleared { get; set; }
    }

    public CraneMonitorService(
        ILogger<CraneMonitorService> logger,
        ICraneService craneService,
        IOpcUaClient opcUaClient,
        IWarehouseQrCodeStore qrCodeStore,
        IStationHandshakeService stationHandshakeService)
    {
        _logger = logger;
        _craneService = craneService;
        _opcUaClient = opcUaClient;
        _qrCodeStore = qrCodeStore;
        _stationHandshakeService = stationHandshakeService;
    }

    /// <summary>
    /// Thiết lập event publisher (gọi từ OpcUaMonitorWorker)
    /// </summary>
    public void SetEventPublisher(Func<object, Task> publisher)
    {
        _eventPublisher = publisher;
    }

    public List<string> GetRequiredCraneTags()
    {
        return new List<string>
        {
            OpcTags.D3051, // ErrorCode
            OpcTags.D3052, // ErrorNote
            OpcTags.D3054, // SpecialError (taskAbnormalFeedback)
            OpcTags.D3050, // ExecuteFeedback
            OpcTags.D3057, // Dispatch/ready after operator clears crane error
            OpcTags.D3053, // TaskCompleteStatus
            OpcTags.D3056, // TaskNo
            OpcTags.D3049, // TaskType
            OpcTags.D3040, // Heartbeat PLC->WCS
            // Confirm data — theo dõi khi TC clear sau ACK D3014
            OpcTags.D3042, OpcTags.D3043, OpcTags.D3044, OpcTags.D3045,
            OpcTags.D3046, OpcTags.D3047, OpcTags.D3048,
            D2022, // CV outbound crane ready → CraneOutboundComplete
            D2027, // CV outbound has box → clear D2321 via D2411 when 1→0
            D2030, // CV inbound has box #2 → clear D2361 via D2451 when 1→0
            D2361  // CV inbound QR ready → CraneInboundReady
        };
    }

    public Task InitializeCraneHeartbeatAsync()
        => WriteCraneHeartbeatAsync(1, "initialize", force: true);

    public async Task ProcessCraneHeartbeatAsync(string nodeId, DataValue dataValue)
    {
        if (nodeId != OpcTags.D3040)
        {
            return;
        }

        try
        {
            var plcHeartbeat = dataValue.Value != null ? Convert.ToInt32(dataValue.Value) : 0;
            var nextWcsHeartbeat = plcHeartbeat == 0 ? 1 : 0;

            await WriteCraneHeartbeatAsync(nextWcsHeartbeat, $"D3040={plcHeartbeat}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lá»—i khi xá»­ lÃ½ crane heartbeat cho node {NodeId}", nodeId);
        }
    }

    public async Task ProcessCraneTaskCompletionAsync(string nodeId, DataValue dataValue)
    {
        try
        {
            // Lưu giá trị mới
            lock (_lockObject)
            {
                _lastValues[nodeId] = dataValue;
            }

            CraneConfirmData? taskCompletionConfirmData = null;

            if (nodeId == OpcTags.D3053)
            {
                var completeStatus = (TaskCompleteStatus)Convert.ToInt32(dataValue.Value);
                if (completeStatus != TaskCompleteStatus.None)
                {
                    taskCompletionConfirmData = await _craneService.ReadConfirmDataAsync();
                }

                await _craneService.HandleTaskFinishStateChangedAsync(completeStatus);
                _logger.LogInformation(
                    "D3053 → D3013={Affirm}, ClearTaskData={Clear}",
                    (int)completeStatus,
                    completeStatus != TaskCompleteStatus.None);
            }

            // Xử lý task completion (D3053 và D3056)
            if (nodeId == OpcTags.D3053 || nodeId == OpcTags.D3056)
            {
                DataValue? currentTaskValue;
                DataValue? completeStatusValue;
                
                lock (_lockObject)
                {
                    if (nodeId == OpcTags.D3056)
                    {
                        currentTaskValue = dataValue;
                        completeStatusValue = _lastValues.GetValueOrDefault(OpcTags.D3053);
                    }
                    else // nodeId == OpcTags.D3053
                    {
                        currentTaskValue = _lastValues.GetValueOrDefault(OpcTags.D3056);
                        completeStatusValue = dataValue;
                    }
                }

                if (currentTaskValue != null && completeStatusValue != null)
                {
                    var currentTask = taskCompletionConfirmData is { TaskNo: > 0 }
                        ? taskCompletionConfirmData.TaskNo
                        : Convert.ToInt32(currentTaskValue.Value);
                    var taskType = taskCompletionConfirmData is { TaskType: not CraneTaskType.None }
                        ? taskCompletionConfirmData.TaskType
                        : GetTaskType() ?? CraneTaskType.None;
                    var completeStatus = Convert.ToInt32(completeStatusValue.Value);

                    if (completeStatus != (int)TaskCompleteStatus.None)
                    {
                        // Publish event
                        await PublishCraneTaskCompletedEventAsync(
                            taskNo: currentTask,
                            taskType: taskType,
                            completeStatus: (TaskCompleteStatus)completeStatus,
                            errorCode: GetErrorCode(),
                            errorNote: GetErrorNote(),
                            confirmData: taskCompletionConfirmData
                        );
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xử lý crane task completion cho node {NodeId}", nodeId);
        }
    }

    public async Task ProcessCvAgvSignalAsync(string nodeId, DataValue oldValue, DataValue newValue)
    {
        try
        {
            if (nodeId != D2027 && nodeId != D2030 && nodeId != D2022 && nodeId != D2361)
            {
                return;
            }

            var oldIntValue = oldValue.Value != null ? Convert.ToInt32(oldValue.Value) : 0;
            var newIntValue = newValue.Value != null ? Convert.ToInt32(newValue.Value) : 0;

            if (nodeId == D2027)
            {
                if (oldIntValue == 1 && newIntValue == 0)
                {
                    _logger.LogInformation(
                        "D2027 1→0 – hàng rời cửa kho xuất, set D2411=0");
                    await _stationHandshakeService.HandleOutboundHasBoxRemovedAsync(CancellationToken.None);
                }

                return;
            }

            if (nodeId == D2030)
            {
                if (oldIntValue == 1 && newIntValue == 0)
                {
                    _logger.LogInformation(
                        "D2030 1→0 – crane đã bốc hàng khỏi cửa nhập, ghi D2451=1 rồi clear sau 4s");
                    await _stationHandshakeService.HandleInboundHasBoxRemovedAsync(CancellationToken.None);
                }

                return;
            }

            if (oldIntValue != 0 || newIntValue != 1)
            {
                return;
            }

            if (nodeId == D2022)
            {
                await PublishCraneOutboundCompleteEventAsync();
            }
            else
            {
                await PublishCraneInboundReadyEventAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xử lý CV↔AGV signal cho node {NodeId}", nodeId);
        }
    }

    public async Task ProcessCraneErrorAsync(string nodeId, DataValue oldValue, DataValue newValue)
    {
        try
        {
            // Lưu giá trị mới
            lock (_lockObject)
            {
                _lastValues[nodeId] = newValue;
            }

            // Bước 6: TC clear confirm data — hoàn tất handshake nếu đang chờ
            if (IsConfirmDataTag(nodeId))
            {
                if (_pendingSpecialErrorAck != null)
                {
                    await TryCompleteSpecialErrorHandshakeAsync();
                }
                return;
            }

            // Chỉ xử lý các tag liên quan đến lỗi
            if (nodeId != OpcTags.D3051 && nodeId != OpcTags.D3052 && nodeId != OpcTags.D3054 && nodeId != OpcTags.D3050 && nodeId != OpcTags.D3057)
            {
                return;
            }

            var oldIntValue = oldValue.Value != null ? Convert.ToInt32(oldValue.Value) : 0;
            var newIntValue = newValue.Value != null ? Convert.ToInt32(newValue.Value) : 0;

            // Chỉ publish khi có lỗi mới xuất hiện (từ 0 hoặc None sang giá trị lỗi)
            if (oldIntValue == newIntValue)
            {
                return;
            }

            // Xử lý ErrorCode (D3051) — publish ngay
            if (nodeId == OpcTags.D3051 && newIntValue != 0)
            {
                await PublishCraneErrorEventAsync(
                    errorCode: newIntValue,
                    errorNote: GetErrorNote(),
                    taskNo: GetTaskNo(),
                    taskType: GetTaskType(),
                    errorSource: "ErrorCode"
                );
            }
            // D3054: handshake TC↔WCS (bước 1–7), publish event ở bước 8
            else if (nodeId == OpcTags.D3054)
            {
                if (newIntValue != 0)
                {
                    await HandleSpecialErrorRaisedAsync((CraneSpecialErrorType)newIntValue);
                }
                else if (oldIntValue != 0)
                {
                    await TryCompleteSpecialErrorHandshakeAsync();
                }
            }
            // 3. Xử lý ExecuteFeedback (D2050) - InstructionError
            else if (nodeId == OpcTags.D3050)
            {
                var executeFeedback = (TaskExecuteFeedback)newIntValue;
                if (executeFeedback == TaskExecuteFeedback.InstructionError)
                {
                    await PublishCraneErrorEventAsync(
                        errorCode: GetErrorCode(),
                        errorNote: GetErrorNote(),
                        executeFeedback: executeFeedback,
                        taskNo: GetTaskNo(),
                        taskType: GetTaskType(),
                        errorSource: "ExecuteFeedback"
                    );
                }
            }
            else if (nodeId == OpcTags.D3057 && newIntValue == 1)
            {
                await TryCompleteSpecialErrorAfterDispatchReadyAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xử lý crane error cho node {NodeId}", nodeId);
        }
    }

    private async Task WriteCraneHeartbeatAsync(int value, string reason, bool force = false)
    {
        await _heartbeatLock.WaitAsync();
        try
        {
            if (!force && _lastHeartbeatWritten == value)
            {
                return;
            }

            if (!force)
            {
                await Task.Delay(CraneHeartbeatDelayMs);
            }

            await _opcUaClient.WriteValueAsync(OpcTags.D3000, value);
            _lastHeartbeatWritten = value;

            _logger.LogDebug("Crane heartbeat {Reason}: ghi D3000={Value}", reason, value);
        }
        finally
        {
            _heartbeatLock.Release();
        }
    }

    private int GetErrorCode()
    {
        try
        {
            lock (_lockObject)
            {
                if (_lastValues.TryGetValue(OpcTags.D3051, out var errorCodeValue))
                {
                    return Convert.ToInt32(errorCodeValue.Value);
                }
                return -1;
            }
        }
        catch
        {
            return -1;
        }
    }

    private string? GetErrorNote()
    {
        try
        {
            lock (_lockObject)
            {
                if (_lastValues.TryGetValue(OpcTags.D3052, out var errorNoteValue))
                {
                    return errorNoteValue.Value?.ToString();
                }
                return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private int GetTaskNo()
    {
        try
        {
            lock (_lockObject)
            {
                if (_lastValues.TryGetValue(OpcTags.D3056, out var taskNoValue))
                {
                    return Convert.ToInt32(taskNoValue.Value);
                }
                return 0;
            }
        }
        catch
        {
            return 0;
        }
    }

    private static bool IsConfirmDataTag(string nodeId)
        => Array.Exists(ConfirmDataTags, t => t == nodeId);

    /// <summary>
    /// Bước 2–4: TC set D3054 → WCS đọc confirm data, ghi D3014 = D3054.
    /// </summary>
    private async Task HandleSpecialErrorRaisedAsync(CraneSpecialErrorType specialError)
    {
        var confirmData = await _craneService.ReadConfirmDataAsync();

        var pending = _pendingSpecialErrorAck;
        if (pending is { ErrorAffirmCleared: true })
        {
            _logger.LogWarning(
                "Ignore D3054={SpecialError} while pending special error awaits D3057. PendingTaskNo={TaskNo}",
                specialError,
                pending.TaskNo);
            return;
        }

        if (confirmData.IsCleared)
        {
            _logger.LogWarning(
                "Ignore D3054={SpecialError} because confirm data is already cleared (TaskNo=0)",
                specialError);
            return;
        }

        _pendingSpecialErrorAck = new PendingSpecialErrorAck
        {
            SpecialError = specialError,
            TaskNo = confirmData.TaskNo,
            TaskType = confirmData.TaskType,
            ConfirmData = confirmData,
            ErrorCode = GetErrorCode(),
            ErrorNote = GetErrorNote()
        };

        await _craneService.WriteTaskErrorAffirmAsync((int)specialError);

        await PublishCraneErrorEventAsync(
            errorCode: _pendingSpecialErrorAck.ErrorCode,
            errorNote: _pendingSpecialErrorAck.ErrorNote,
            specialError: specialError,
            confirmData: confirmData,
            taskNo: confirmData.TaskNo,
            taskType: confirmData.TaskType,
            errorSource: "SpecialErrorRaised"
        );

        _logger.LogWarning(
            "D3054 lỗi đặc biệt={SpecialError}: confirm TaskNo={TaskNo}, TaskType={TaskType}, Tunnel={Tunnel} → ghi D3014={Affirm}",
            specialError, confirmData.TaskNo, confirmData.TaskType, confirmData.Tunnel, (int)specialError);
    }

    /// <summary>
    /// Bước 6–7: TC clear D3054 và confirm data → WCS ghi D3014=0, giữ pending đến khi D3057=1.
    /// </summary>
    private async Task TryCompleteSpecialErrorHandshakeAsync()
    {
        var pending = _pendingSpecialErrorAck;
        if (pending == null)
        {
            return;
        }

        int d3054;
        lock (_lockObject)
        {
            d3054 = _lastValues.TryGetValue(OpcTags.D3054, out var v)
                ? Convert.ToInt32(v.Value)
                : -1;
        }

        if (d3054 != 0)
        {
            return;
        }

        var confirmData = await _craneService.ReadConfirmDataAsync();
        if (!confirmData.IsCleared)
        {
            return;
        }

        await _craneService.WriteTaskErrorAffirmAsync(0);
        pending.ErrorAffirmCleared = true;

        _logger.LogWarning(
            "D3054 handshake hoàn tất: D3054=0, confirm cleared → D3014=0. TaskNo={TaskNo}",
            pending.TaskNo);
        _logger.LogWarning("Pending special error is kept until D3057=1. TaskNo={TaskNo}", pending.TaskNo);

        return;
    }

    private async Task TryCompleteSpecialErrorAfterDispatchReadyAsync()
    {
        var pending = _pendingSpecialErrorAck;
        if (pending == null || !pending.ErrorAffirmCleared)
        {
            return;
        }

        _pendingSpecialErrorAck = null;

        _logger.LogWarning("D3057=1 after special error: clear pending. TaskNo={TaskNo}", pending.TaskNo);

        await PublishCraneErrorEventAsync(
            errorCode: pending.ErrorCode,
            errorNote: pending.ErrorNote,
            specialError: pending.SpecialError,
            confirmData: pending.ConfirmData,
            taskNo: pending.TaskNo,
            taskType: pending.TaskType,
            errorSource: "SpecialError"
        );
    }

    private CraneTaskType? GetTaskType()
    {
        try
        {
            lock (_lockObject)
            {
                if (_lastValues.TryGetValue(OpcTags.D3049, out var taskTypeValue))
                {
                    return (CraneTaskType)Convert.ToInt32(taskTypeValue.Value);
                }
                return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private async Task PublishCraneErrorEventAsync(
        int errorCode,
        string? errorNote = null,
        CraneSpecialErrorType? specialError = null,
        TaskExecuteFeedback? executeFeedback = null,
        CraneConfirmData? confirmData = null,
        int taskNo = 0,
        CraneTaskType? taskType = null,
        string errorSource = "ErrorCode")
    {
        try
        {
            if (_eventPublisher == null)
            {
                _logger.LogWarning("EventPublisher chưa được thiết lập, không thể publish CraneErrorEvent");
                return;
            }

            var errorEvent = new CraneErrorEvent(
                errorCode: errorCode,
                errorNote: errorNote,
                specialError: specialError,
                executeFeedback: executeFeedback,
                confirmData: confirmData,
                taskNo: taskNo,
                taskType: taskType,
                errorSource: errorSource
            );

            _logger.LogWarning("🚨 Crane Error: Source={Source}, ErrorCode={ErrorCode}, SpecialError={SpecialError}, ExecuteFeedback={ExecuteFeedback}, TaskNo={TaskNo}",
                errorSource, errorCode, specialError, executeFeedback, taskNo);

            await _eventPublisher(errorEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi publish CraneErrorEvent");
        }
    }

    private async Task PublishCraneInboundReadyEventAsync()
    {
        try
        {
            if (_eventPublisher == null)
            {
                _logger.LogWarning("EventPublisher chưa được thiết lập, không thể publish CraneInboundReadyEvent");
                return;
            }

            var qrCode = _qrCodeStore.GetInboundQr();
            var readyEvent = new CraneInboundReadyEvent(qrCode);

            _logger.LogInformation("📥 Crane Inbound Ready: D2361 0→1, QrCode={QrCode}", qrCode ?? "(null)");

            await _eventPublisher(readyEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi publish CraneInboundReadyEvent");
        }
    }

    private async Task PublishCraneOutboundCompleteEventAsync()
    {
        try
        {
            if (_eventPublisher == null)
            {
                _logger.LogWarning("EventPublisher chưa được thiết lập, không thể publish CraneOutboundCompleteEvent");
                return;
            }

            var qrCode = _qrCodeStore.GetOutboundQr();
            var completeEvent = new CraneOutboundCompleteEvent(qrCode);

            _logger.LogInformation("📤 Crane Outbound Complete: D2022 0→1, QrCode={QrCode}", qrCode ?? "(null)");

            await _eventPublisher(completeEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi publish CraneOutboundCompleteEvent");
        }
    }

    private async Task PublishCraneTaskCompletedEventAsync(
        int taskNo,
        CraneTaskType taskType,
        TaskCompleteStatus completeStatus,
        int errorCode = 0,
        string? errorNote = null,
        CraneConfirmData? confirmData = null)
    {
        try
        {
            if (_eventPublisher == null)
            {
                _logger.LogWarning("EventPublisher chưa được thiết lập, không thể publish CraneTaskCompletedEvent");
                return;
            }

            var completedEvent = new CraneTaskCompletedEvent(
                taskNo: taskNo,
                taskType: taskType,
                completeStatus: completeStatus,
                errorCode: errorCode,
                errorNote: errorNote,
                confirmData: confirmData
            );

            _logger.LogInformation("✅ Crane Task Completed: TaskNo={TaskNo}, Status={Status}, Type={Type}",
                taskNo, completeStatus, taskType);

            await _eventPublisher(completedEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi publish CraneTaskCompletedEvent");
        }
    }
}

