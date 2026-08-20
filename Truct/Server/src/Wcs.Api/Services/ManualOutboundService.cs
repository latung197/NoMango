using Wcs.Api.Controllers.DTOs;
using Wcs.Okamura;
using Wcs.Okamura.Models;
using Wcs.Okamura.Services;
using Wcs.OpcUa.Contracts;

namespace Wcs.Api.Services;

public interface IManualOutboundService
{
    Task<ManualOutboundResult> SetManualModeAsync(int open, int timeoutSeconds, CancellationToken ct);
    Task<ManualOutboundStatusResult> GetStatusAsync(CancellationToken ct);
}

public sealed class ManualOutboundService(
    IStationHandshakeService handshake,
    IOpcUaClient opc,
    ILogger<ManualOutboundService> logger) : IManualOutboundService
{
    private const int ValidMinutes = 5;

    private readonly IStationHandshakeService _handshake = handshake;
    private readonly IOpcUaClient _opc = opc;
    private readonly ILogger<ManualOutboundService> _logger = logger;
    private readonly object _lock = new();
    private CancellationTokenSource? _autoCloseCts;

    public Task<ManualOutboundResult> SetManualModeAsync(int open, int timeoutSeconds, CancellationToken ct)
    {
        return open switch
        {
            1 => OpenAsync(timeoutSeconds, ct),
            0 => CloseAsync(ct, reason: "manual"),
            _ => throw new ArgumentOutOfRangeException(nameof(open), "open phải là 0 hoặc 1"),
        };
    }

    public async Task<ManualOutboundStatusResult> GetStatusAsync(CancellationToken ct)
    {
        try
        {
            var dataValue = await _opc.ReadValueAsync(OpcTagsCvAgv.D2026);
            ct.ThrowIfCancellationRequested();

            var isGood = Opc.Ua.StatusCode.IsGood(dataValue.StatusCode);
            if (!isGood)
            {
                return new ManualOutboundStatusResult
                {
                    NodeId = OpcTagsCvAgv.D2026,
                    IsGood = false,
                    Error = dataValue.StatusCode.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Description = "Không đọc được D2026",
                };
            }

            var status = Convert.ToInt32(dataValue.Value);
            return new ManualOutboundStatusResult
            {
                NodeId = OpcTagsCvAgv.D2026,
                SafetySensorStatus = status,
                IsManualMode = status == 1,
                Description = status == 1 ? "Rèm tắt (chế độ thủ công)" : "Rèm bật",
                Timestamp = dataValue.SourceTimestamp == DateTime.MinValue ? DateTime.UtcNow : dataValue.SourceTimestamp,
                IsGood = true,
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Outbound Manual] Lỗi đọc D2026");
            return new ManualOutboundStatusResult
            {
                NodeId = OpcTagsCvAgv.D2026,
                IsGood = false,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow,
                Description = "Lỗi đọc OPC",
            };
        }
    }

    private async Task<ManualOutboundResult> OpenAsync(int timeoutSeconds, CancellationToken ct)
    {
        CancelAutoCloseTimer();

        string? qrCode = null;
        var steps = new List<string>();

        if (await IsOutboundQrReadyTagAsync(ct))
        {
            _logger.LogInformation("[Outbound Manual] D2321=1, đọc QR code");
            qrCode = await _handshake.Outbound_ReadQrCodeAsync(ct);

            _logger.LogInformation("[Outbound Manual] Đọc QR xong, QrCode={QrCode}", qrCode);
            steps.Add("Outbound_ReadQrCode");
        }
        else
        {
            _logger.LogInformation("[Outbound Manual] D2321≠1, bỏ qua đọc QR");
        }

        _logger.LogInformation("[Outbound Manual] Bật chế độ thủ công – BeforePick (kiểm tra ngay, không chờ)");
        await _handshake.Outbound_BeforePickAsync(TimeSpan.FromSeconds(timeoutSeconds), ct, waitForCvReady: false);

        // _logger.LogInformation("[Outbound Manual] EnterPickupPoint");
        // await _handshake.Outbound_EnterPickupPointAsync(ct);

        // _logger.LogInformation("[Outbound Manual] LiftRack");
        // await _handshake.Outbound_LiftRackAsync(ct);

        var expiresAt = DateTime.UtcNow.AddMinutes(ValidMinutes);
        ScheduleAutoClose(expiresAt);

        var safetyStatus = await ReadOutboundSafetySensorStatusAsync(ct);
        _logger.LogInformation(
            "[Outbound Manual] Chế độ thủ công đã bật. D2026={Status}, hết hạn {ExpiresAt:O}",
            safetyStatus, expiresAt);

        return new ManualOutboundResult
        {
            Open = 1,
            SafetySensorStatus = safetyStatus,
            ExpiresAt = expiresAt,
            ValidSeconds = ValidMinutes * 60,
            QrCode = qrCode,
            StepsCompleted =
            [
                ..steps,
                "Outbound_BeforePick",
                "Outbound_EnterPickupPoint",
                "Outbound_LiftRack",
            ],
        };
    }

    private async Task<ManualOutboundResult> CloseAsync(CancellationToken ct, string reason)
    {
        CancelAutoCloseTimer();

        _logger.LogInformation("[Outbound Manual] Tắt chế độ thủ công ({Reason}) – BackToPickupWaitingPoint", reason);
        await _handshake.Outbound_BackToPickupWaitingPointAsync(ct);

        _logger.LogInformation("[Outbound Manual] AfterPickup");
        await _handshake.Outbound_AfterPickupAsync(ct);

        var safetyStatus = await ReadOutboundSafetySensorStatusAsync(ct);
        _logger.LogInformation("[Outbound Manual] Chế độ thủ công đã tắt. D2026={Status}", safetyStatus);

        return new ManualOutboundResult
        {
            Open = 0,
            SafetySensorStatus = safetyStatus,
            StepsCompleted =
            [
                "Outbound_BackToPickupWaitingPoint",
                "Outbound_AfterPickup",
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

            if (!await IsOutboundCurtainMutedAsync(ct))
            {
                _logger.LogInformation("[Outbound Manual] Hết hạn 5 phút – rèm đã đóng (D2026=0), bỏ qua auto-close");
                return;
            }

            _logger.LogWarning("[Outbound Manual] Hết hạn 5 phút – rèm vẫn mute (D2026=1), tự đóng tag AGV");
            await CloseAsync(CancellationToken.None, reason: "timeout");
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Outbound Manual] Lỗi auto-close sau 5 phút");
        }
    }

    private async Task<bool> IsOutboundCurtainMutedAsync(CancellationToken ct)
        => await ReadOutboundSafetySensorStatusAsync(ct) == 1;

    private async Task<bool> IsOutboundQrReadyTagAsync(CancellationToken ct)
    {
        var value = await _opc.ReadValueAsync(OpcTagsCvAgv.D2321);
        ct.ThrowIfCancellationRequested();
        return Convert.ToInt32(value.Value) == 1;
    }

    private async Task<int> ReadOutboundSafetySensorStatusAsync(CancellationToken ct)
    {
        var value = await _opc.ReadValueAsync(OpcTagsCvAgv.D2026);
        ct.ThrowIfCancellationRequested();
        return Convert.ToInt32(value.Value);
    }
}
