namespace Wcs.Okamura.Services;

using Opc.Ua;

/// <summary>
/// Interface để xử lý monitoring crane qua OPC tags
/// </summary>
public interface ICraneMonitorService
{
    /// <summary>
    /// Xử lý thay đổi giá trị OPC tag liên quan đến crane completion (D2053, D2056)
    /// Publish CraneTaskCompletedEvent khi task hoàn thành
    /// </summary>
    Task ProcessCraneTaskCompletionAsync(string nodeId, DataValue dataValue);
    
    /// <summary>
    /// Xử lý thay đổi giá trị OPC tag liên quan đến crane error (D2051, D2052, D2054, D2050)
    /// Publish CraneErrorEvent khi có lỗi
    /// </summary>
    Task ProcessCraneErrorAsync(string nodeId, DataValue oldValue, DataValue newValue);

    /// <summary>
    /// Xử lý tín hiệu CV↔AGV: D2022 (CraneOutboundComplete), D2027 (clear QR khi hết hàng), D2361 (CraneInboundReady).
    /// </summary>
    Task ProcessCvAgvSignalAsync(string nodeId, DataValue oldValue, DataValue newValue);
    
    /// <summary>
    /// Lấy danh sách các OPC tags liên quan đến crane cần subscribe
    /// </summary>
    List<string> GetRequiredCraneTags();

    Task InitializeCraneHeartbeatAsync();

    Task ProcessCraneHeartbeatAsync(string nodeId, DataValue dataValue);
    
    /// <summary>
    /// Thiết lập event publisher (gọi từ OpcUaMonitorWorker)
    /// </summary>
    void SetEventPublisher(Func<object, Task> publisher);
}
