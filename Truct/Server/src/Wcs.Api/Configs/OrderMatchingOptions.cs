namespace Wcs.Api.Configs;

public sealed class OrderMatchingOptions
{
    public int SendRequestTimeoutMinutes { get; set; } = 10;
    public int PollingIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Khi true: auto send vẫn tạo lệnh dù OPC không đọc được QR (dùng size trạm).
    /// </summary>
    public bool SkipQrCode { get; set; }

    public TimeSpan GetSendTimeout() =>
        TimeSpan.FromMinutes(SendRequestTimeoutMinutes <= 0 ? 10 : SendRequestTimeoutMinutes);

    public TimeSpan GetPollingInterval()
    {
        var seconds = PollingIntervalSeconds <= 0 ? 30 : PollingIntervalSeconds;
        return TimeSpan.FromSeconds(Math.Max(10, seconds));
    }

    public bool DispatchingRecoveryEnabled { get; set; } = true;

    public int DispatchingStuckTimeoutMinutes { get; set; } = 5;

    public int DispatchingRecoveryPollingIntervalSeconds { get; set; } = 60;

    public TimeSpan GetDispatchingStuckTimeout() =>
        TimeSpan.FromMinutes(DispatchingStuckTimeoutMinutes <= 0 ? 5 : DispatchingStuckTimeoutMinutes);

    public TimeSpan GetDispatchingRecoveryPollingInterval()
    {
        var seconds = DispatchingRecoveryPollingIntervalSeconds <= 0 ? 60 : DispatchingRecoveryPollingIntervalSeconds;
        return TimeSpan.FromSeconds(Math.Max(30, seconds));
    }
}
