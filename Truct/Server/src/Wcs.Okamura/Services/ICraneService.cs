namespace Wcs.Okamura.Services;

using Wcs.Okamura.Models;
using Wcs.Okamura.Enums;

public interface ICraneService
{
    /// <summary>
    /// Gửi task nhập kho (Inbound). Chờ crane sẵn sàng (D3057=1, D3050/D3053/D3051=0) trước khi gửi.
    /// </summary>
    Task<CraneTaskResult> SendInboundAsync(InboundTask task, CancellationToken ct = default, bool waitForReady = true);

    /// <summary>
    /// Gửi task xuất kho (Outbound). Chờ crane sẵn sàng trước khi gửi; các task xuất đồng thời được xếp hàng tuần tự.
    /// </summary>
    Task<CraneTaskResult> SendOutboundAsync(OutboundTask task, CancellationToken ct = default, bool waitForReady = true);

    /// <summary>
    /// Gửi task di chuyển nội bộ trong kho (Internal Rack Move).
    /// </summary>
    Task<CraneTaskResult> SendInternalMoveAsync(InternalMoveTask task, CancellationToken ct = default, bool waitForReady = true);

    /// <summary>
    /// Gửi task di chuyển giữa các station.
    /// </summary>
    Task<CraneTaskResult> SendStationMoveAsync(StationMoveTask task, CancellationToken ct = default, bool waitForReady = true);

    /// <summary>
    /// Đọc trạng thái sẵn sàng nhận lệnh mới (D3050, D3053, D3051, D3057).
    /// </summary>
    Task<CraneReadyStatus> GetReadyStatusAsync(CancellationToken ct = default);

    /// <summary>
    /// Chờ đến khi crane sẵn sàng hoặc hết timeout.
    /// </summary>
    Task<bool> WaitUntilReadyAsync(TimeSpan timeout, CancellationToken ct = default);

    /// <summary>
    /// Hủy tác vụ hiện tại (Manual Cancel hoặc Force Cancel).
    /// </summary>
    Task<bool> CancelCurrentTaskAsync(int taskNo);

    /// <summary>
    /// Xóa toàn bộ dữ liệu instruction task đã gửi xuống PLC (D3003–D3010, D3016).
    /// </summary>
    Task ClearTaskDataAsync();

    /// <summary>
    /// Ghi xác nhận hoàn thành task (D3013 / wcs_taskFinishAffirm).
    /// </summary>
    Task WriteTaskFinishAffirmAsync(int value);

    /// <summary>
    /// Ghi xác nhận lỗi task (D3014 / wcs_taskErrorAffirm). TC nhận ACK rồi clear D3054 và confirm data.
    /// </summary>
    Task WriteTaskErrorAffirmAsync(int value);

    /// <summary>
    /// Đọc confirm data PLC→PC (D3042–D3049, D3056).
    /// </summary>
    Task<CraneConfirmData> ReadConfirmDataAsync();

    /// <summary>
    /// Xử lý khi D3053 (taskFinishState) thay đổi: ghi D3013 tương ứng và clear task data nếu ≠ 0.
    /// </summary>
    Task HandleTaskFinishStateChangedAsync(TaskCompleteStatus status);

    /// <summary>
    /// Kiểm tra trạng thái hiện tại của Crane (tổng hợp từ PLC → PC).
    /// </summary>
    Task<CraneFeedback> GetCurrentFeedbackAsync();

    /// <summary>
    /// Đọc trạng thái vị trí hiện tại của Crane (Column, Level, Fork).
    /// </summary>
    Task<CranePosition> GetCranePositionAsync();
}