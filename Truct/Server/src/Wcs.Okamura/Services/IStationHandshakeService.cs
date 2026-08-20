namespace Wcs.Okamura.Services;

using Wcs.Okamura.Models;
using Wcs.Okamura.Enums;

/// <summary>
/// Station handshake service cho CV ↔ AGV
/// - Không phụ thuộc stationCode
/// - Không phụ thuộc business context
/// - Chỉ phản ánh đúng protocol Okamura
/// </summary>
public interface IStationHandshakeService
{
    // ==================================================
    // OUTBOUND (Xuất kho) = PICK side
    // ==================================================

    /// <summary>
    /// Step: BeforePick
    /// - AGV Ready
    /// - Wait CV Ready + AllowHandover + Safety OK
    /// </summary>
    Task Outbound_BeforePickAsync(TimeSpan timeout, CancellationToken ct, bool waitForCvReady = true);

    /// <summary>
    /// Step: EnterPickupPoint
    /// - AGV Arrived
    /// </summary>
    Task Outbound_EnterPickupPointAsync(CancellationToken ct);

    /// <summary>
    /// Step: LiftRack
    /// - AGV HandoverInProgress
    /// </summary>
    Task Outbound_LiftRackAsync(CancellationToken ct);

    /// <summary>
    /// Step: BackToPickupWaitingPoint
    /// - Clear HandoverInProgress
    /// - Clear Arrived
    /// </summary>
    Task Outbound_BackToPickupWaitingPointAsync(CancellationToken ct);

    /// <summary>
    /// Step: AfterPickup
    /// - No mandatory OPC action (hook point)
    /// </summary>
    Task Outbound_AfterPickupAsync(CancellationToken ct);

    /// <summary>
    /// Read QR code from outbound CV.
    /// - Read D2311..D2320 and merge to one string
    /// - Write D2411=1 when read success, D2411=2 when data invalid/read fails
    /// - Cache the value for display/event only
    /// </summary>
    Task<string?> Outbound_ReadQrCodeAsync(CancellationToken ct);

    /// <summary>
    /// Chờ crane hạ hàng xuống băng tải outbound (D2022 = 1).
    /// Dùng sau khi crane xuất hàng ra station; chưa đồng nghĩa CV đã đưa hàng ra vị trí AGV.
    /// </summary>
    Task WaitOutboundCraneReadyAsync(TimeSpan timeout, CancellationToken ct);

    /// <summary>Đọc nhanh D2022 – crane đã hạ hàng xuống băng tải.</summary>
    Task<bool> IsOutboundCraneReadyAsync(CancellationToken ct);

    /// <summary>
    /// Chờ cảm biến vật lý báo có hàng tại cửa kho xuất (D2027 = 1).
    /// Dùng sau <see cref="WaitOutboundCraneReadyAsync"/>, trước khi đọc QR.
    /// </summary>
    Task WaitOutboundHasBoxAsync(TimeSpan timeout, CancellationToken ct);

    /// <summary>Đọc nhanh D2027 – có hàng tại cửa kho xuất.</summary>
    Task<bool> IsOutboundHasBoxAsync(CancellationToken ct);

    /// <summary>Kiểm tra ngay D2027=1, không chờ.</summary>
    Task<OpcConditionCheckResult> CheckOutboundHasBoxAsync(CancellationToken ct);

    /// <summary>
    /// Chờ PLC báo QR đã quét xong, sẵn sàng cho WCS đọc (D2321 = 1).
    /// Dùng sau <see cref="WaitOutboundHasBoxAsync"/>, trước <see cref="Outbound_ReadQrCodeAsync"/>.
    /// </summary>
    Task WaitOutboundQrReadyAsync(TimeSpan timeout, CancellationToken ct);

    /// <summary>Đọc nhanh D2321 – QR sẵn sàng đọc (không phải tín hiệu có hàng).</summary>
    Task<bool> IsOutboundQrReadyAsync(CancellationToken ct);

    /// <summary>
    /// Xử lý khi D2027 chuyển 1→0 (hàng đã rời cửa kho xuất): set D2411=0,
    /// rồi xóa outbound QR cache.
    /// </summary>
    Task HandleOutboundHasBoxRemovedAsync(CancellationToken ct);

    /// <summary>
    /// Set D2411=0 để PLC clear chu kỳ QR outbound.
    /// Chỉ gọi qua <see cref="HandleOutboundHasBoxRemovedAsync"/> khi D2027 chuyển 1→0.
    /// </summary>
    Task CompleteOutboundQrAsync(CancellationToken ct);


    // ==================================================
    // INBOUND (Nhập kho) = DROP side
    // ==================================================

    /// <summary>
    /// Step: BeforeDrop
    /// - AGV Ready
    /// - Wait CV Ready + AllowHandover + Safety OK + no box at door (D2028=0)
    /// </summary>
    Task Inbound_BeforeDropAsync(
        TimeSpan timeout,
        CancellationToken ct,
        bool waitForCvReady = true);

    /// <summary>Kiểm tra ngay D2011+D2012+D2013, không chờ.</summary>
    Task<OpcConditionCheckResult> CheckOutboundCvReadyAsync(CancellationToken ct);

    /// <summary>Kiểm tra ngay D2001+D2002+D2003+D2028=0, không chờ.</summary>
    Task<OpcConditionCheckResult> CheckInboundCvReadyAsync(CancellationToken ct);

    /// <summary>Kiểm tra ngay D2022=1, không chờ.</summary>
    Task<OpcConditionCheckResult> CheckOutboundCraneReadyAsync(CancellationToken ct);

    /// <summary>Kiểm tra ngay D2021=1, không chờ.</summary>
    Task<OpcConditionCheckResult> CheckInboundCraneReadyAsync(CancellationToken ct);

    /// <summary>Kiểm tra ngay D2361=1, không chờ.</summary>
    Task<OpcConditionCheckResult> CheckInboundQrReadyAsync(CancellationToken ct);

    /// <summary>
    /// Step: EnterDropPoint
    /// - AGV Arrived
    /// </summary>
    Task Inbound_EnterDropPointAsync(CancellationToken ct);

    /// <summary>
    /// Step: PutdownRack
    /// - AGV Receiving (đang đặt hàng)
    /// </summary>
    Task Inbound_PutdownRackAsync(CancellationToken ct);

    /// <summary>
    /// Step: BackToDropWaitingPoint
    /// - Clear Receiving
    /// - Clear Arrived
    /// </summary>
    Task Inbound_BackToDropWaitingPointAsync(CancellationToken ct);

    /// <summary>
    /// Step: AfterDrop
    /// - Clear AGV Arrived (D2102=0)
    /// - Optionally wait CV CraneReady (D2021=1) – cassette sẵn sàng cho crane lấy
    /// </summary>
    Task Inbound_AfterDropAsync(
        CancellationToken ct,
        bool waitCraneReady = true);

    /// <summary>
    /// Read QR code from inbound CV.
    /// - Read D2351..D2360 and merge to one string
    /// - Cache the value for display/event only; clear cache when data invalid/read fails
    /// - Does not write D2451; D2451 is written when D2030 transitions 1→0
    /// </summary>
    Task<string?> Inbound_ReadQrCodeAsync(CancellationToken ct);

    /// <summary>
    /// Xử lý khi D2030 chuyển 1→0 (crane đã bốc hàng khỏi cửa nhập):
    /// luôn ghi D2451=1, sau 4s clear D2451=0.
    /// </summary>
    Task HandleInboundHasBoxRemovedAsync(CancellationToken ct);

    /// <summary>
    /// Set D2451=0 để PLC clear chu kỳ QR inbound.
    /// </summary>
    Task CompleteInboundQrAsync(CancellationToken ct);

    /// <summary>
    /// Chờ CV đọc QR sau khi AGV thả hàng – D2361 = 1.
    /// Dùng sau <see cref="Inbound_AfterDropAsync"/>, trước <see cref="Inbound_ReadQrCodeAsync"/>.
    /// </summary>
    Task WaitInboundQrReadyAsync(TimeSpan timeout, CancellationToken ct);

    /// <summary>Đọc nhanh D2361 – QR sẵn sàng đọc.</summary>
    Task<bool> IsInboundQrReadyAsync(CancellationToken ct);

    /// <summary>
    /// Chờ CV báo cassette đã ở vị trí sẵn sàng cho crane lấy (D2021 = 1).
    /// Dùng sau AGV thả hàng và CV đưa cassette vào, trước khi gửi lệnh crane inbound.
    /// </summary>
    Task WaitInboundCraneReadyAsync(TimeSpan timeout, CancellationToken ct);

    /// <summary>Đọc nhanh D2021 – cassette sẵn sàng cho crane lấy.</summary>
    Task<bool> IsInboundCraneReadyAsync(CancellationToken ct);

    /// <summary>
    /// Reset toàn bộ tín hiệu handshake băng tải kho (cả inbound lẫn outbound):
    /// BackToWaiting → AfterDrop/AfterPickup, và đưa D2411/D2451 về 0.
    /// </summary>
    Task ResetWarehouseConveyorSignalsAsync(CancellationToken ct);
}
