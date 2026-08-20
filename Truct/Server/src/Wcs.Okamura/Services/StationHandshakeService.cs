namespace Wcs.Okamura.Services;

using Wcs.Okamura.Models;
using Wcs.Okamura.Enums;
using Wcs.OpcUa.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public sealed class StationHandshakeService(
    IOpcUaClient opc,
    IWarehouseQrCodeStore qrCodeStore,
    IServiceScopeFactory scopeFactory,
    ILogger<StationHandshakeService> logger,
    TimeSpan? pollInterval = null) : IStationHandshakeService
{
    private readonly IOpcUaClient _opc = opc ?? throw new ArgumentNullException(nameof(opc));
    private readonly IWarehouseQrCodeStore _qrCodeStore = qrCodeStore ?? throw new ArgumentNullException(nameof(qrCodeStore));
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    private readonly ILogger<StationHandshakeService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly TimeSpan _pollInterval = pollInterval ?? TimeSpan.FromMilliseconds(150);

    // ==================================================
    // OUTBOUND (Xuất kho) = PICK side  => use INBOUND tag set (D200x/D210x)
    // ==================================================

    public async Task Outbound_BeforePickAsync(TimeSpan timeout, CancellationToken ct, bool waitForCvReady = true)
    {
        // WRITE: AGV Ready
        await WriteBoolAsync(OpcTagsCvAgv.D2112, true, ct);

        if (waitForCvReady)
        {
            await WaitUntilAsync(async x =>
            {
                var readyOk = await ReadBoolAsync(OpcTagsCvAgv.D2011, x);
                var alowAllowHandoverOk = await ReadBoolAsync(OpcTagsCvAgv.D2012, x);
                var safetyOk = await ReadBoolAsync(OpcTagsCvAgv.D2013, x);
                return readyOk && alowAllowHandoverOk && safetyOk;
            }, timeout, ct);
            return;
        }

        var check = await CheckOutboundCvReadyAsync(ct);
        if (!check.IsSatisfied) {
            await WriteBoolAsync(OpcTagsCvAgv.D2112, false, ct);
            throw new OpcConditionsNotMetException(check);
        }
    }

    public async Task Outbound_EnterPickupPointAsync(CancellationToken ct)
    {
        await WriteBoolAsync(OpcTagsCvAgv.D2113, true, ct);
        await WriteBoolAsync(OpcTagsCvAgv.D2114, true, ct);
    }

    public Task Outbound_LiftRackAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public async Task Outbound_BackToPickupWaitingPointAsync(CancellationToken ct)
    {
        // Clear in-progress first, then clear arrived
        await WriteBoolAsync(OpcTagsCvAgv.D2113, false, ct);
        await WriteBoolAsync(OpcTagsCvAgv.D2114, false, ct);
    }

    public async Task Outbound_AfterPickupAsync(CancellationToken ct)
    {
        await WriteBoolAsync(OpcTagsCvAgv.D2112, false, ct);
    }

    public Task WaitOutboundCraneReadyAsync(TimeSpan timeout, CancellationToken ct)
    {
        return WaitUntilAsync(
            x => ReadBoolAsync(OpcTagsCvAgv.D2022, x),
            timeout,
            ct,
            $"Timed out after {timeout.TotalSeconds:0}s waiting for outbound crane ready ({OpcTagsCvAgv.D2022}).");
    }

    public Task<bool> IsOutboundCraneReadyAsync(CancellationToken ct)
        => ReadBoolAsync(OpcTagsCvAgv.D2022, ct);

    public Task WaitOutboundHasBoxAsync(TimeSpan timeout, CancellationToken ct)
    {
        return WaitUntilAsync(
            async x => await ReadIntAsync(OpcTagsCvAgv.D2027, x) == 1,
            timeout,
            ct,
            $"Timed out after {timeout.TotalSeconds:0}s waiting for outbound has box ({OpcTagsCvAgv.D2027}=1).");
    }

    public async Task<bool> IsOutboundHasBoxAsync(CancellationToken ct)
        => await ReadIntAsync(OpcTagsCvAgv.D2027, ct) == 1;

    public Task WaitOutboundQrReadyAsync(TimeSpan timeout, CancellationToken ct)
    {
        return WaitUntilAsync(
            async x => await IsOutboundQrReadyFromOpcAsync(x),
            timeout,
            ct,
            $"Timed out after {timeout.TotalSeconds:0}s waiting for outbound QR ready ({OpcTagsCvAgv.D2321}=1).");
    }

    public async Task<bool> IsOutboundQrReadyAsync(CancellationToken ct)
        => await IsOutboundQrReadyFromOpcAsync(ct);

    public async Task HandleOutboundHasBoxRemovedAsync(CancellationToken ct)
        => await CompleteOutboundQrAsync(ct);

    public async Task CompleteOutboundQrAsync(CancellationToken ct)
    {
        await _opc.WriteValueAsync(OpcTagsCvAgv.D2411, 0);
        _qrCodeStore.SetOutboundQr(null);
    }

    public async Task<string?> Outbound_ReadQrCodeAsync(CancellationToken ct)
    {
        try
        {
            // D2311–D2320 được OPC server expose thành 1 node block duy nhất
            var qrCode = await ReadStringAsync(OpcTagsCvAgv.D2311_2320, ct);
            if (string.IsNullOrWhiteSpace(qrCode))
            {
                await _opc.WriteValueAsync(OpcTagsCvAgv.D2411, 2);
                _qrCodeStore.SetOutboundQr(null);
                return null;
            }

            await _opc.WriteValueAsync(OpcTagsCvAgv.D2411, 1);
            _qrCodeStore.SetOutboundQr(qrCode);
            return qrCode;
        }
        catch
        {
            await _opc.WriteValueAsync(OpcTagsCvAgv.D2411, 2);
            return null;
        }
    }

    // ==================================================
    // INBOUND (Nhập kho) = DROP side => use OUTBOUND tag set (D201x/D211x)
    // ==================================================

    public async Task Inbound_BeforeDropAsync(
        TimeSpan timeout,
        CancellationToken ct,
        bool waitForCvReady = true)
    {
        // WRITE: AGV Ready
        await WriteBoolAsync(OpcTagsCvAgv.D2102, true, ct);
        await WriteBoolAsync(OpcTagsCvAgv.D2101, true, ct);

        if (waitForCvReady)
        {
            await WaitUntilAsync(async x =>
            {
                var readyOk = await ReadBoolAsync(OpcTagsCvAgv.D2001, x);
                var alowAllowHandoverOk = await ReadBoolAsync(OpcTagsCvAgv.D2002, x);
                var safetyOk = await ReadBoolAsync(OpcTagsCvAgv.D2003, x);
                var noBoxOk = await ReadIntAsync(OpcTagsCvAgv.D2028, x) == 0;
                return readyOk && alowAllowHandoverOk && safetyOk && noBoxOk;
            }, timeout, ct);
            return;
        }

        var check = await CheckInboundCvReadyAsync(ct);
        if (!check.IsSatisfied)
            throw new OpcConditionsNotMetException(check);
    }

    public async Task Inbound_EnterDropPointAsync(CancellationToken ct)
    {
        await WriteBoolAsync(OpcTagsCvAgv.D2103, true, ct);
        await WriteBoolAsync(OpcTagsCvAgv.D2104, true, ct);
    }

    public Task Inbound_PutdownRackAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public async Task Inbound_BackToDropWaitingPointAsync(CancellationToken ct)
    {
        // Clear receiving first, then clear arrived
        await WriteBoolAsync(OpcTagsCvAgv.D2103, false, ct);
        await WriteBoolAsync(OpcTagsCvAgv.D2104, false, ct);
    }

    public async Task Inbound_AfterDropAsync(
        CancellationToken ct,
        bool waitCraneReady = true)
    {
        await WriteBoolAsync(OpcTagsCvAgv.D2102, false, ct);

        if (waitCraneReady)
            await WaitInboundCraneReadyAsync(TimeSpan.FromSeconds(120), ct);
    }

    public Task WaitInboundQrReadyAsync(TimeSpan timeout, CancellationToken ct)
    {
        return WaitUntilAsync(
            async x => await IsInboundQrReadyFromOpcAsync(x),
            timeout,
            ct,
            $"Timed out after {timeout.TotalSeconds:0}s waiting for inbound QR ready ({OpcTagsCvAgv.D2361}=1).");
    }

    public async Task<bool> IsInboundQrReadyAsync(CancellationToken ct)
        => await IsInboundQrReadyFromOpcAsync(ct);

    public async Task<string?> Inbound_ReadQrCodeAsync(CancellationToken ct)
    {
        var qrCode = await ReadInboundQrFromOpcAsync(ct);
        if (qrCode is null)
        {
            _qrCodeStore.SetInboundQr(null);
            return null;
        }

        _qrCodeStore.SetInboundQr(qrCode);
        return qrCode;
    }

    public async Task HandleInboundHasBoxRemovedAsync(CancellationToken ct)
    {
        const int d2451Value = 1;
        int? taskNo = null;

        using (var scope = _scopeFactory.CreateScope())
        {
            var store = scope.ServiceProvider.GetService<ICraneInboundQrFinishedDispatchStore>();
            taskNo = store is null
                ? null
                : await store.TryGetActiveInboundTaskNoAsync(ct);

            if (store is null)
            {
                _logger.LogWarning("ICraneInboundQrFinishedDispatchStore not registered; skip DB record for D2451");
            }
            else if (taskNo is null)
            {
                _logger.LogWarning(
                    "No active inbound crane dispatch for D2451; Value={Value}",
                    d2451Value);
            }
        }

        _logger.LogInformation(
            "Write D2451={Value} (TaskNo={TaskNo}) after D2030 1→0",
            d2451Value,
            taskNo);

        await _opc.WriteValueAsync(OpcTagsCvAgv.D2451, d2451Value);
        _qrCodeStore.SetInboundQr(null);

        if (taskNo is int raisedTaskNo)
        {
            using var scope = _scopeFactory.CreateScope();
            var store = scope.ServiceProvider.GetService<ICraneInboundQrFinishedDispatchStore>();
            if (store is not null)
            {
                await store.RecordD2451RaisedAsync(raisedTaskNo, ct);
            }
        }

        await Task.Delay(TimeSpan.FromSeconds(4), ct);

        await _opc.WriteValueAsync(OpcTagsCvAgv.D2451, 0);

        _logger.LogInformation(
            "Write D2451=0 (TaskNo={TaskNo}) after 4s pulse",
            taskNo);

        if (taskNo is int clearedTaskNo)
        {
            using var scope = _scopeFactory.CreateScope();
            var store = scope.ServiceProvider.GetService<ICraneInboundQrFinishedDispatchStore>();
            if (store is not null)
            {
                await store.RecordD2451ClearedAsync(clearedTaskNo, ct);
            }
        }
    }

    public async Task CompleteInboundQrAsync(CancellationToken ct)
    {
        await _opc.WriteValueAsync(OpcTagsCvAgv.D2451, 0);
        _qrCodeStore.SetInboundQr(null);
    }

    public Task WaitInboundCraneReadyAsync(TimeSpan timeout, CancellationToken ct)
    {
        return WaitUntilAsync(
            x => ReadBoolAsync(OpcTagsCvAgv.D2021, x),
            timeout,
            ct);
    }

    public Task<bool> IsInboundCraneReadyAsync(CancellationToken ct)
        => ReadBoolAsync(OpcTagsCvAgv.D2021, ct);

    public async Task ResetWarehouseConveyorSignalsAsync(CancellationToken ct)
    {
        await Inbound_BackToDropWaitingPointAsync(ct);
        await Inbound_AfterDropAsync(ct, waitCraneReady: false);

        await Outbound_BackToPickupWaitingPointAsync(ct);
        await Outbound_AfterPickupAsync(ct);

        await _opc.WriteValueAsync(OpcTagsCvAgv.D2411, 0);
        await _opc.WriteValueAsync(OpcTagsCvAgv.D2451, 0);
        _qrCodeStore.SetOutboundQr(null);
        _qrCodeStore.SetInboundQr(null);
    }

    public async Task<OpcConditionCheckResult> CheckOutboundCvReadyAsync(CancellationToken ct)
    {
        var failures = new List<OpcTagFailure>();
        await AddBoolFailureIfNeeded(failures, OpcTagsCvAgv.D2011, "CV Ready", ct);
        await AddBoolFailureIfNeeded(failures, OpcTagsCvAgv.D2012, "CV AllowHandover", ct);
        await AddBoolFailureIfNeeded(failures, OpcTagsCvAgv.D2013, "CV Safety OK", ct);
        return OpcConditionCheckResult.FromFailures(failures);
    }

    public async Task<OpcConditionCheckResult> CheckInboundCvReadyAsync(CancellationToken ct)
    {
        var failures = new List<OpcTagFailure>();
        await AddBoolFailureIfNeeded(failures, OpcTagsCvAgv.D2001, "CV Ready", ct);
        await AddBoolFailureIfNeeded(failures, OpcTagsCvAgv.D2002, "CV AllowHandover", ct);
        await AddBoolFailureIfNeeded(failures, OpcTagsCvAgv.D2003, "CV Safety OK", ct);
        await AddIntFailureIfNotExpected(failures, OpcTagsCvAgv.D2028, "Inbound has no box (D2028)", expected: 0, ct);
        return OpcConditionCheckResult.FromFailures(failures);
    }

    public async Task<OpcConditionCheckResult> CheckOutboundCraneReadyAsync(CancellationToken ct)
        => await CheckBoolTagAsync(OpcTagsCvAgv.D2022, "CV CraneReady outbound", ct);

    public async Task<OpcConditionCheckResult> CheckOutboundHasBoxAsync(CancellationToken ct)
        => await CheckBoolTagAsync(OpcTagsCvAgv.D2027, "Outbound has box (D2027)", ct);

    public async Task<OpcConditionCheckResult> CheckInboundCraneReadyAsync(CancellationToken ct)
        => await CheckBoolTagAsync(OpcTagsCvAgv.D2021, "CV CraneReady inbound", ct);

    public Task<OpcConditionCheckResult> CheckInboundQrReadyAsync(CancellationToken ct)
        => CheckIntTagAsync(OpcTagsCvAgv.D2361, "QR Ready", expected: 1, ct);

    // ==================================================
    // Internal helpers
    // ==================================================

    private async Task<OpcConditionCheckResult> CheckBoolTagAsync(string tag, string description, CancellationToken ct)
    {
        var actual = await ReadIntAsync(tag, ct);
        if (actual != 0)
            return OpcConditionCheckResult.Ok;

        return OpcConditionCheckResult.FromFailures([
            new OpcTagFailure(tag, description, "1", actual.ToString()),
        ]);
    }

    private async Task<OpcConditionCheckResult> CheckIntTagAsync(
        string tag,
        string description,
        int expected,
        CancellationToken ct)
    {
        var actual = await ReadIntAsync(tag, ct);
        if (actual == expected)
            return OpcConditionCheckResult.Ok;

        return OpcConditionCheckResult.FromFailures([
            new OpcTagFailure(tag, description, expected.ToString(), actual.ToString()),
        ]);
    }

    private async Task AddBoolFailureIfNeeded(
        List<OpcTagFailure> failures,
        string tag,
        string description,
        CancellationToken ct)
    {
        var actual = await ReadIntAsync(tag, ct);
        if (actual == 0)
            failures.Add(new OpcTagFailure(tag, description, "1", actual.ToString()));
    }

    private async Task AddIntFailureIfNotExpected(
        List<OpcTagFailure> failures,
        string tag,
        string description,
        int expected,
        CancellationToken ct)
    {
        var actual = await ReadIntAsync(tag, ct);
        if (actual != expected)
            failures.Add(new OpcTagFailure(tag, description, expected.ToString(), actual.ToString()));
    }

    private async Task<bool> ReadBoolAsync(string tag, CancellationToken ct)
    {
        var value = await _opc.ReadValueAsync(tag);
        return Convert.ToInt32(value.Value) != 0;
    }

    private Task WriteBoolAsync(string tag, bool value, CancellationToken ct)
    {
        return _opc.WriteValueAsync(tag, value ? 1 : 0);
    }

    private async Task<int> ReadIntAsync(string tag, CancellationToken ct)
    {
        var value = await _opc.ReadValueAsync(tag);
        return Convert.ToInt32(value.Value);
    }

    private async Task<string?> ReadInboundQrFromOpcAsync(CancellationToken ct)
    {
        try
        {
            // D2351–D2360 được OPC server expose thành 1 node block duy nhất
            var qrCode = await ReadStringAsync(OpcTagsCvAgv.D2351_2360, ct);
            return string.IsNullOrWhiteSpace(qrCode) ? null : qrCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read inbound QR from OPC block {Tag}", OpcTagsCvAgv.D2351_2360);
            return null;
        }
    }

    private async Task<string> ReadStringAsync(string tag, CancellationToken ct)
    {
        var value = await _opc.ReadValueAsync(tag);
        var text = Convert.ToString(value.Value);
        return text ?? throw new InvalidOperationException($"Cannot read string from tag {tag}.");
    }

    private Task<bool> IsInboundQrReadyFromOpcAsync(CancellationToken ct)
        => ReadBoolAsync(OpcTagsCvAgv.D2361, ct);

    private Task<bool> IsOutboundQrReadyFromOpcAsync(CancellationToken ct)
        => ReadBoolAsync(OpcTagsCvAgv.D2321, ct);

    private async Task WaitUntilAsync(
        Func<CancellationToken, Task<bool>> condition,
        TimeSpan timeout,
        CancellationToken ct,
        string? timeoutMessage = null)
    {
        if (timeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be > 0.");

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        try
        {
            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();

                if (await condition(cts.Token))
                    return;

                await Task.Delay(_pollInterval, cts.Token);
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException(
                timeoutMessage ?? $"Timed out after {timeout.TotalSeconds:0}s waiting for OPC condition.");
        }
    }
}
