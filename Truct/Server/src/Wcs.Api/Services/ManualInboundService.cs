using Wcs.Api.Controllers.DTOs;
using Wcs.Okamura;
using Wcs.Okamura.Services;
using Wcs.OpcUa.Contracts;

namespace Wcs.Api.Services;

public interface IManualInboundService
{
    Task<ManualInboundResult> SetManualModeAsync(int open, int timeoutSeconds, CancellationToken ct);
    Task<ManualInboundStatusResult> GetStatusAsync(CancellationToken ct);
}

public sealed class ManualInboundService(
    IStationHandshakeService handshake,
    IOpcUaClient opc,
    ILogger<ManualInboundService> logger) : IManualInboundService
{
    private const int ValidMinutes = 5;

    private readonly IStationHandshakeService _handshake = handshake;
    private readonly IOpcUaClient _opc = opc;
    private readonly ILogger<ManualInboundService> _logger = logger;
    private readonly object _lock = new();
    private CancellationTokenSource? _autoCloseCts;

    public Task<ManualInboundResult> SetManualModeAsync(int open, int timeoutSeconds, CancellationToken ct)
    {
        return open switch
        {
            1 => OpenAsync(timeoutSeconds, ct),
            0 => CloseAsync(ct, waitCraneReady: false, reason: "manual"),
            _ => throw new ArgumentOutOfRangeException(nameof(open), "open phải là 0 hoặc 1"),
        };
    }

    public async Task<ManualInboundStatusResult> GetStatusAsync(CancellationToken ct)
    {
        try
        {
            var dataValue = await _opc.ReadValueAsync(OpcTagsCvAgv.D2024);
            ct.ThrowIfCancellationRequested();

            var isGood = Opc.Ua.StatusCode.IsGood(dataValue.StatusCode);
            if (!isGood)
            {
                return new ManualInboundStatusResult
                {
                    NodeId = OpcTagsCvAgv.D2024,
                    IsGood = false,
                    Error = dataValue.StatusCode.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Description = "Không đọc được D2024",
                };
            }

            var status = Convert.ToInt32(dataValue.Value);
            return new ManualInboundStatusResult
            {
                NodeId = OpcTagsCvAgv.D2024,
                SafetySensorStatus = status,
                IsManualMode = status == 1,
                Description = status == 1 ? "Rèm tắt (chế độ thủ công)" : "Rèm bật",
                Timestamp = dataValue.SourceTimestamp == DateTime.MinValue ? DateTime.UtcNow : dataValue.SourceTimestamp,
                IsGood = true,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Inbound Manual] Lỗi đọc D2024");
            return new ManualInboundStatusResult
            {
                NodeId = OpcTagsCvAgv.D2024,
                IsGood = false,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow,
                Description = "Lỗi đọc OPC",
            };
        }
    }

    private async Task<ManualInboundResult> OpenAsync(int timeoutSeconds, CancellationToken ct)
    {
        CancelAutoCloseTimer();

        _logger.LogInformation("[Inbound Manual] Bật chế độ thủ công – BeforeDrop (kiểm tra ngay, không chờ)");
        await _handshake.Inbound_BeforeDropAsync(TimeSpan.FromSeconds(timeoutSeconds), ct, waitForCvReady: false);

        // _logger.LogInformation("[Inbound Manual] EnterDropPoint");
        // await _handshake.Inbound_EnterDropPointAsync(ct);

        // _logger.LogInformation("[Inbound Manual] PutdownRack");
        // await _handshake.Inbound_PutdownRackAsync(ct);

        var expiresAt = DateTime.UtcNow.AddMinutes(ValidMinutes);
        ScheduleAutoClose(expiresAt);

        var safetyStatus = await ReadInboundSafetySensorStatusAsync(ct);
        _logger.LogInformation(
            "[Inbound Manual] Chế độ thủ công đã bật. D2024={Status}, hết hạn {ExpiresAt:O}",
            safetyStatus, expiresAt);

        return new ManualInboundResult
        {
            Open = 1,
            SafetySensorStatus = safetyStatus,
            ExpiresAt = expiresAt,
            ValidSeconds = ValidMinutes * 60,
            StepsCompleted =
            [
                "Inbound_BeforeDrop",
            ],
        };
    }

    private async Task<ManualInboundResult> CloseAsync(
        CancellationToken ct,
        bool waitCraneReady,
        string reason)
    {
        CancelAutoCloseTimer();

        _logger.LogInformation("[Inbound Manual] Tắt chế độ thủ công ({Reason}) – BackToDropWaitingPoint", reason);
        await _handshake.Inbound_BackToDropWaitingPointAsync(ct);

        _logger.LogInformation("[Inbound Manual] AfterDrop (waitCraneReady={WaitCraneReady})", waitCraneReady);
        await _handshake.Inbound_AfterDropAsync(ct, waitCraneReady: waitCraneReady);

        var safetyStatus = await ReadInboundSafetySensorStatusAsync(ct);
        _logger.LogInformation("[Inbound Manual] Chế độ thủ công đã tắt. D2024={Status}", safetyStatus);

        return new ManualInboundResult
        {
            Open = 0,
            SafetySensorStatus = safetyStatus,
            StepsCompleted =
            [
                "Inbound_BackToDropWaitingPoint",
                "Inbound_AfterDrop",
            ],
        };
    }

    private void ScheduleAutoClose(DateTime expiresAt)
    {
        var cts = new CancellationTokenSource();
        lock (_lock)
        {
            _autoCloseCts?.Cancel();
            _autoCloseCts?.Dispose();
            _autoCloseCts = cts;
        }

        _ = RunAutoCloseAsync(expiresAt, cts.Token);
    }

    private void CancelAutoCloseTimer()
    {
        lock (_lock)
        {
            _autoCloseCts?.Cancel();
            _autoCloseCts?.Dispose();
            _autoCloseCts = null;
        }
    }

    private async Task RunAutoCloseAsync(DateTime expiresAt, CancellationToken ct)
    {
        try
        {
            var delay = expiresAt - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, ct);

            if (!await IsInboundCurtainMutedAsync(ct))
            {
                _logger.LogInformation("[Inbound Manual] Hết hạn 5 phút – rèm đã đóng (D2024=0), bỏ qua auto-close");
                return;
            }

            _logger.LogWarning("[Inbound Manual] Hết hạn 5 phút – rèm vẫn mute (D2024=1), tự đóng tag AGV");
            await CloseAsync(CancellationToken.None, waitCraneReady: false, reason: "timeout");
        }
        catch (OperationCanceledException)
        {
            // Timer bị hủy do open=0 hoặc open=1 mới
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Inbound Manual] Lỗi auto-close sau 5 phút");
        }
    }

    private async Task<bool> IsInboundCurtainMutedAsync(CancellationToken ct)
        => await ReadInboundSafetySensorStatusAsync(ct) == 1;

    private async Task<int> ReadInboundSafetySensorStatusAsync(CancellationToken ct)
    {
        var value = await _opc.ReadValueAsync(OpcTagsCvAgv.D2024);
        ct.ThrowIfCancellationRequested();
        return Convert.ToInt32(value.Value);
    }
}
