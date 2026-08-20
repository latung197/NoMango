namespace Wcs.Okamura.Services;

/// <summary>
/// Cache QR inbound/outbound sau khi đọc từ OPC qua <see cref="IStationHandshakeService"/>.
/// Chỉ dùng để hiển thị (publish event SignalR); không tham gia check logic chờ/kiểm tra QR ready.
/// </summary>
public interface IWarehouseQrCodeStore
{
    void SetInboundQr(string? qrCode);
    void SetOutboundQr(string? qrCode);
    string? GetInboundQr();
    string? GetOutboundQr();
}
