using Microsoft.Extensions.Options;
using Wcs.Api.Configs;
using Wcs.Api.OrderMatching;
using Wcs.Api.Services;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Workers;

public sealed class AutoStationOrderWorker(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<OrderMatchingOptions> options,
    ILogger<AutoStationOrderWorker> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly OrderMatchingOptions _options = options.Value;
    private readonly ILogger<AutoStationOrderWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "AutoStationOrderWorker started (interval={Interval}s)",
            _options.PollingIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<AutoStationOrderRunner>();
                await runner.ProcessAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AutoStationOrderWorker");
            }

            try
            {
                await Task.Delay(_options.GetPollingInterval(), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("AutoStationOrderWorker stopping");
    }
}

public sealed class AutoStationOrderRunner(
    StationService stationService,
    StationSnapshotService stationSnapshotService,
    TransferMatchingService transferMatchingService,
    ITransferRequestRepository transferRequestRepository,
    ICassetteRepository cassetteRepository,
    IOptions<OrderMatchingOptions> options,
    ILogger<AutoStationOrderRunner> logger)
{
    private readonly StationService _stationService = stationService;
    private readonly StationSnapshotService _stationSnapshotService = stationSnapshotService;
    private readonly TransferMatchingService _transferMatchingService = transferMatchingService;
    private readonly ITransferRequestRepository _transferRequestRepository = transferRequestRepository;
    private readonly ICassetteRepository _cassetteRepository = cassetteRepository;
    private readonly OrderMatchingOptions _options = options.Value;
    private readonly ILogger<AutoStationOrderRunner> _logger = logger;

    public async Task ProcessAsync(CancellationToken ct = default)
    {
        var stations = await _stationService.GetAllStations();
        foreach (var station in stations)
        {
            if (station.Type == InOut.IN && station.AutoReceiveEnabled)
            {
                await TryCreateAutoReceiveAsync(station, ct);
            }
            else if (station.Type == InOut.OUT && station.AutoSendEnabled)
            {
                await TryCreateAutoSendAsync(station, ct);
            }
        }
    }

    private async Task TryCreateAutoReceiveAsync(Station station, CancellationToken ct)
    {
        try
        {
            if (!await _stationService.IsStationFreeForNewTaskAsync(station.Code))
            {
                _logger.LogDebug("Auto IN skipped for {StationCode}: station has active task", station.Code);
                return;
            }

            if (await _transferRequestRepository.ExistsActiveReceiveOnStationAsync(station.Code, ct))
            {
                _logger.LogDebug("Auto IN skipped for {StationCode}: active receive already exists", station.Code);
                return;
            }

            var snapshot = await _stationSnapshotService.BuildAsync(station, ct);
            if (!IsReadyToReceive(snapshot))
            {
                _logger.LogDebug(
                    "Auto IN skipped for {StationCode}: display={DisplayState}, running={IsRunning}",
                    station.Code,
                    snapshot.DisplayState,
                    snapshot.Status.IsRunning);
                return;
            }

            var candidate = await FindWaitingSendForAutoReceiveAsync(station, ct);
            if (candidate is not null)
            {
                await _transferMatchingService.CreateReceiveAsync(new ReceiveTransferRequest
                {
                    ToStation = station.Code,
                    IsEmptyTray = candidate.IsEmptyTray,
                    Size = (int)candidate.Size,
                    Source = TransferRequestSource.AutoOpc,
                }, ct);
                return;
            }

            await TryCreateAutoWarehouseReceiveAsync(station, ct);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogInformation(ex, "Auto IN skipped for {StationCode}: {Reason}", station.Code, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auto IN failed for {StationCode}", station.Code);
        }
    }

    /// <summary>
    /// Không có Send waiting → thử gọi hàng từ kho theo stage của trạm.
    /// Nếu kho không có hàng / không fulfill được thì hủy Receive để không khóa trạm.
    /// </summary>
    private async Task TryCreateAutoWarehouseReceiveAsync(Station station, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(station.StageCode))
        {
            _logger.LogDebug("Auto IN warehouse skipped for {StationCode}: missing stage", station.Code);
            return;
        }

        var sizes = station.Sizes
            .Where(s => Enum.IsDefined(typeof(Size), s))
            .OrderBy(s => (int)s)
            .ToList();
        if (sizes.Count == 0)
        {
            _logger.LogInformation("Auto IN warehouse skipped for {StationCode}: station has no size", station.Code);
            return;
        }

        foreach (var size in sizes)
        {
            var result = await _transferMatchingService.CreateReceiveAsync(new ReceiveTransferRequest
            {
                ToStation = station.Code,
                IsEmptyTray = false,
                Size = (int)size,
                Source = TransferRequestSource.AutoOpc,
            }, ct);

            // Đã tạo FlowTask hoặc đang xếp hàng cửa kho OUT → giữ request.
            if (result.Status == TransferRequestStatus.Waiting ||
                result.FlowTaskId is not null ||
                result.WarehousePendingKind != WarehousePendingKind.None)
            {
                _logger.LogInformation(
                    "Auto IN warehouse for {StationCode}: request {RequestId} size={Size} status={Status}, pending={Pending}",
                    station.Code, result.Id, size, result.Status, result.WarehousePendingKind);
                return;
            }

            // Không có hàng size này → hủy rồi thử size tiếp theo / vòng poll sau.
        }

        _logger.LogDebug(
            "Auto IN warehouse skipped for {StationCode}: no stock for stage {StageCode}",
            station.Code, station.StageCode);
    }

    private async Task<TransferRequest?> FindWaitingSendForAutoReceiveAsync(Station station, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(station.StageCode))
        {
            return null;
        }

        var waiting = await _transferRequestRepository.GetWaitingByStageAsync(station.StageCode, ct);
        return waiting
            .Where(r =>
                r.Type == TransferRequestType.Send &&
                string.Equals(r.ToStageCode, station.StageCode, StringComparison.OrdinalIgnoreCase) &&
                station.SupportsSize(r.Size))
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefault();
    }

    private async Task TryCreateAutoSendAsync(Station station, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(station.AutoSendToStage))
            {
                _logger.LogInformation("Auto OUT skipped for {StationCode}: missing destination stage", station.Code);
                return;
            }

            if (!await _stationService.IsStationFreeForNewTaskAsync(station.Code))
            {
                _logger.LogDebug("Auto OUT skipped for {StationCode}: station has active task", station.Code);
                return;
            }

            var snapshot = await _stationSnapshotService.BuildAsync(station, ct);
            if (!IsReadyForPickup(snapshot))
            {
                _logger.LogDebug(
                    "Auto OUT skipped for {StationCode}: display={DisplayState}, running={IsRunning}",
                    station.Code,
                    snapshot.DisplayState,
                    snapshot.Status.IsRunning);
                return;
            }

            if (station.AutoSendIsEmptyTray)
            {
                await TryCreateAutoEmptyTraySendAsync(station, ct);
                return;
            }

            await TryCreateAutoGoodsSendAsync(station, snapshot, ct);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogInformation(ex, "Auto OUT skipped for {StationCode}: {Reason}", station.Code, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auto OUT failed for {StationCode}", station.Code);
        }
    }

    private async Task TryCreateAutoEmptyTraySendAsync(Station station, CancellationToken ct)
    {
        var size = station.Sizes.OrderBy(s => (int)s).FirstOrDefault();
        if (!Enum.IsDefined(typeof(Size), size))
        {
            _logger.LogInformation("Auto OUT empty tray skipped for {StationCode}: station has no size", station.Code);
            return;
        }

        if (await HasDuplicateSendAsync(station, size, true, ct))
        {
            _logger.LogDebug("Auto OUT empty tray skipped for {StationCode}: duplicate waiting send", station.Code);
            return;
        }

        await _transferMatchingService.CreateSendAsync(new SendTransferRequest
        {
            FromStation = station.Code,
            ToStage = station.AutoSendToStage!,
            IsEmptyTray = true,
            Size = (int)size,
            Source = TransferRequestSource.AutoOpc,
        }, ct);
    }

    private async Task TryCreateAutoGoodsSendAsync(Station station, StationSnapshot snapshot, CancellationToken ct)
    {
        var qrCode = snapshot.Tray.QrCode?.Trim();
        if (string.IsNullOrWhiteSpace(qrCode))
        {
            if (!_options.SkipQrCode)
            {
                _logger.LogInformation("Auto OUT goods skipped for {StationCode}: missing QR code", station.Code);
                return;
            }

            await TryCreateAutoGoodsSendWithoutQrAsync(station, ct);
            return;
        }

        var cassette = await _cassetteRepository.GetActiveByCodeAsync(qrCode, ct);
        if (cassette is null)
        {
            if (_options.SkipQrCode)
            {
                _logger.LogInformation(
                    "Auto OUT goods for {StationCode}: cassette {CassetteCode} not found, SkipQrCode enabled — create without cassette",
                    station.Code,
                    qrCode);
                await TryCreateAutoGoodsSendWithoutQrAsync(station, ct);
                return;
            }

            _logger.LogInformation(
                "Auto OUT goods skipped for {StationCode}: cassette {CassetteCode} not found",
                station.Code,
                qrCode);
            return;
        }

        if (!station.SupportsSize(cassette.Size))
        {
            _logger.LogInformation(
                "Auto OUT goods skipped for {StationCode}: cassette {CassetteCode} size {Size} is not supported",
                station.Code,
                cassette.Code,
                cassette.Size);
            return;
        }

        if (await HasDuplicateSendAsync(station, cassette.Size, false, cassette.Code, ct))
        {
            _logger.LogDebug("Auto OUT goods skipped for {StationCode}: duplicate waiting send", station.Code);
            return;
        }

        await _transferMatchingService.CreateSendAsync(new SendTransferRequest
        {
            FromStation = station.Code,
            ToStage = station.AutoSendToStage!,
            IsEmptyTray = false,
            CassetteCode = cassette.Code,
            Product = cassette.Product,
            Quantity = cassette.Quantity,
            StorageStageCode = station.AutoSendToStage,
            Size = (int)cassette.Size,
            Source = TransferRequestSource.AutoOpc,
        }, ct);
    }

    private async Task TryCreateAutoGoodsSendWithoutQrAsync(Station station, CancellationToken ct)
    {
        var size = station.Sizes.OrderBy(s => (int)s).FirstOrDefault();
        if (!Enum.IsDefined(typeof(Size), size))
        {
            _logger.LogInformation("Auto OUT goods skipped for {StationCode}: station has no size (SkipQrCode)", station.Code);
            return;
        }

        if (await HasDuplicateSendAsync(station, size, false, ct))
        {
            _logger.LogDebug("Auto OUT goods skipped for {StationCode}: duplicate waiting send (SkipQrCode)", station.Code);
            return;
        }

        _logger.LogInformation(
            "Auto OUT goods for {StationCode}: creating send without QR (SkipQrCode), size={Size}",
            station.Code,
            size);

        await _transferMatchingService.CreateSendAsync(new SendTransferRequest
        {
            FromStation = station.Code,
            ToStage = station.AutoSendToStage!,
            IsEmptyTray = false,
            StorageStageCode = station.AutoSendToStage,
            Size = (int)size,
            Source = TransferRequestSource.AutoOpc,
        }, ct);
    }

    private Task<bool> HasDuplicateSendAsync(Station station, Size size, bool isEmptyTray, CancellationToken ct) =>
        HasDuplicateSendAsync(station, size, isEmptyTray, null, ct);

    private Task<bool> HasDuplicateSendAsync(
        Station station,
        Size size,
        bool isEmptyTray,
        string? cassetteCode,
        CancellationToken ct) =>
        _transferRequestRepository.ExistsActiveAsync(
            TransferRequestType.Send,
            station.StageCode,
            station.AutoSendToStage!,
            size,
            isEmptyTray,
            station.Code,
            cassetteCode,
            ct);

    private static bool IsReadyToReceive(StationSnapshot snapshot) =>
        snapshot.IsActive &&
        snapshot.DisplayState == StationDisplayState.NoCassette &&
        snapshot.Conveyor.IsFull != true &&
        snapshot.Conveyor.CanAcceptDrop != false &&
        snapshot.Status.IsRunning != false;

    private static bool IsReadyForPickup(StationSnapshot snapshot) =>
        snapshot.IsActive &&
        snapshot.DisplayState == StationDisplayState.HasCassette &&
        snapshot.Status.IsRunning != false;
}
