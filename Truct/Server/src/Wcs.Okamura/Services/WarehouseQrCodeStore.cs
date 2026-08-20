namespace Wcs.Okamura.Services;

public sealed class WarehouseQrCodeStore : IWarehouseQrCodeStore
{
    private readonly object _lock = new();
    private string? _inboundQr;
    private string? _outboundQr;

    public void SetInboundQr(string? qrCode)
    {
        lock (_lock)
        {
            _inboundQr = string.IsNullOrEmpty(qrCode) ? null : qrCode;
        }
    }

    public void SetOutboundQr(string? qrCode)
    {
        lock (_lock)
        {
            _outboundQr = string.IsNullOrEmpty(qrCode) ? null : qrCode;
        }
    }

    public string? GetInboundQr()
    {
        lock (_lock)
        {
            return _inboundQr;
        }
    }

    public string? GetOutboundQr()
    {
        lock (_lock)
        {
            return _outboundQr;
        }
    }
}
