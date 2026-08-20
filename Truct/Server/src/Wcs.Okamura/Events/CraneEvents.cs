using Wcs.Okamura.Enums;
using Wcs.Okamura.Models;

namespace Wcs.Okamura.Events;

/// <summary>
/// Event được publish khi có lỗi từ crane
/// </summary>
public class CraneErrorEvent
{
    public int TaskNo { get; set; }
    public CraneTaskType? TaskType { get; set; }
    public int ErrorCode { get; set; }
    public string? ErrorNote { get; set; }
    public CraneSpecialErrorType? SpecialError { get; set; }
    public TaskExecuteFeedback? ExecuteFeedback { get; set; }
    public CraneConfirmData? ConfirmData { get; set; }
    public string ErrorSource { get; set; } = string.Empty; // "ErrorCode", "SpecialErrorRaised", "SpecialError", "ExecuteFeedback"
    public DateTime Timestamp { get; set; }
    
    public static string EventType => "CraneError";
    
    public CraneErrorEvent()
    {
        Timestamp = DateTime.UtcNow;
    }
    
    public CraneErrorEvent(
        int errorCode,
        string? errorNote = null,
        CraneSpecialErrorType? specialError = null,
        TaskExecuteFeedback? executeFeedback = null,
        CraneConfirmData? confirmData = null,
        int taskNo = 0,
        CraneTaskType? taskType = null,
        string errorSource = "ErrorCode")
    {
        TaskNo = taskNo;
        TaskType = taskType;
        ErrorCode = errorCode;
        ErrorNote = errorNote;
        SpecialError = specialError;
        ExecuteFeedback = executeFeedback;
        ConfirmData = confirmData;
        ErrorSource = errorSource;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Event được publish khi crane task hoàn thành
/// </summary>
public class CraneTaskCompletedEvent
{
    public int TaskNo { get; set; }
    public CraneTaskType TaskType { get; set; }
    public TaskCompleteStatus CompleteStatus { get; set; }
    public int ErrorCode { get; set; }
    public string? ErrorNote { get; set; }
    public CraneConfirmData? ConfirmData { get; set; }
    public DateTime Timestamp { get; set; }
    
    public static string EventType => "CraneTaskCompleted";
    
    public CraneTaskCompletedEvent(
        int taskNo,
        CraneTaskType taskType,
        TaskCompleteStatus completeStatus,
        int errorCode = 0,
        string? errorNote = null,
        CraneConfirmData? confirmData = null)
    {
        TaskNo = taskNo;
        TaskType = taskType;
        CompleteStatus = completeStatus;
        ErrorCode = errorCode;
        ErrorNote = errorNote;
        ConfirmData = confirmData;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Event được publish khi D2361 chuyển từ 0 → 1 (CV inbound QR ready).
/// </summary>
public class CraneInboundReadyEvent
{
    /// <summary>QR inbound (D2351–D2360) đọc được từ kho.</summary>
    public string? QrCode { get; set; }
    public DateTime Timestamp { get; set; }

    public static string EventType => "CraneInboundReady";

    public CraneInboundReadyEvent(string? qrCode = null)
    {
        QrCode = qrCode;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Event được publish khi D2022 chuyển từ 0 → 1 (CV outbound sẵn sàng cho crane đặt hàng).
/// </summary>
public class CraneOutboundCompleteEvent
{
    /// <summary>QR outbound (D2311–D2320) đọc được từ kho.</summary>
    public string? QrCode { get; set; }
    public DateTime Timestamp { get; set; }

    public static string EventType => "CraneOutboundComplete";

    public CraneOutboundCompleteEvent(string? qrCode = null)
    {
        QrCode = qrCode;
        Timestamp = DateTime.UtcNow;
    }
}



