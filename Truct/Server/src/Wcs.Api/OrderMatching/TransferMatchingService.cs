using Microsoft.Extensions.Options;

using Wcs.Api.Configs;

using Wcs.Api.Services;

using Wcs.Common.Abstractions;

using Wcs.Common.Abstractions.Repositories;

using Wcs.Common.Entities;

using Wcs.Common.Events;

using Wcs.Common.Exceptions;

using Wcs.Common.Extensions;

using Wcs.Common.ValueObjects;



namespace Wcs.Api.OrderMatching;



public sealed class TransferMatchingService(

    ITransferRequestRepository transferRequestRepository,

    StationService stationService,

    FlowTaskStarter flowTaskStarter,

    IWmsService wmsService,

    IEventPublisher publisher,

    IOptions<OrderMatchingOptions> options,

    IOptions<WarehouseOptions> warehouseOptions,

    ILogger<TransferMatchingService> logger)

{

    private readonly ITransferRequestRepository _transferRequestRepository = transferRequestRepository;

    private readonly StationService _stationService = stationService;

    private readonly FlowTaskStarter _flowTaskStarter = flowTaskStarter;

    private readonly IWmsService _wmsService = wmsService;

    private readonly IEventPublisher _publisher = publisher;

    private readonly OrderMatchingOptions _options = options.Value;

    private readonly WarehouseOptions _warehouseOptions = warehouseOptions.Value;

    private readonly ILogger<TransferMatchingService> _logger = logger;



    public async Task<TransferRequest> CreateSendAsync(SendTransferRequest input, CancellationToken ct = default)

    {

        var fromStation = await RequireStationAsync(input.FromStation, ct);

        var fromStage = RequireStage(fromStation, input.FromStation);

        var toStage = input.ToStage;

        var size = StationSizeResolver.ResolveForStation(fromStation, input.Size);



        await RequireDestinationStageSupportsSizeAsync(toStage, size, ct);

        await RequireNoDuplicateSendAsync(
            fromStage,
            toStage,
            size,
            input.IsEmptyTray,
            fromStation.Code,
            input.CassetteCode,
            ct);

        await RequireNoConcurrentSendOnStationAsync(fromStation.Code, ct);

        if (!await _stationService.IsStationFreeForNewTaskAsync(fromStation.Code))
        {
            throw new InvalidOperationException(
                "Trạm đang có lệnh robot thực hiện, không thể tạo yêu cầu gửi mới");
        }

        var request = await _transferRequestRepository.CreateAsync(new TransferRequest

        {

            Type = TransferRequestType.Send,

            FromStageCode = fromStage,

            ToStageCode = toStage,

            Size = size,

            FromStationCode = fromStation.Code,

            IsEmptyTray = input.IsEmptyTray,

            CassetteCode = input.CassetteCode,

            Product = input.Product,

            Quantity = input.Quantity,

            StorageStageCode = input.StorageStageCode,

            RobotCode = input.RobotCode,

            Source = input.Source,

            Status = TransferRequestStatus.Waiting,

            ExpiresAt = DateTime.UtcNow.Add(_options.GetSendTimeout()),

        }, ct);

        await PublishRequestChangedAsync(request, ct);

        _logger.LogInformation(

            "Send request {RequestId} waiting: {FromStation} ({FromStage}) -> {ToStage}, expires {ExpiresAt}",

            request.Id, fromStation.Code, fromStage, toStage, request.ExpiresAt);

        return request;



    }



    /// <summary>Receive: khớp Send → thử kho → chờ.</summary>

    public async Task<TransferRequest> CreateReceiveAsync(ReceiveTransferRequest input, CancellationToken ct = default)

    {

        var toStation = await RequireStationAsync(input.ToStation, ct);

        var toStage = RequireStage(toStation, input.ToStation);

        var size = StationSizeResolver.ResolveForStation(toStation, input.Size);



        await RequireNoConcurrentReceiveOnStationAsync(toStation.Code, ct);

        if (!await _stationService.IsStationFreeForNewTaskAsync(toStation.Code))
        {
            throw new InvalidOperationException(
                "Trạm đang có lệnh robot thực hiện, không thể tạo yêu cầu nhận mới");
        }

        await _stationService.ValidateStationCanAcceptDropAsync(toStation);



        string fromStageCode;

        if (input.IsWipTray)

        {

            if (input.IsEmptyTray)

            {

                throw new InvalidOperationException("is_wip_tray không được dùng cùng is_empty_tray");

            }



            fromStageCode = toStage;

        }

        else if (input.IsEmptyTray)

        {

            fromStageCode = string.Empty;

        }

        else

        {

            fromStageCode = input.Source == TransferRequestSource.AutoOpc
                ? string.Empty
                : RequireFromStage(input.FromStage);

        }



        var request = await _transferRequestRepository.CreateAsync(new TransferRequest

        {

            Type = TransferRequestType.Receive,

            FromStageCode = fromStageCode,

            ToStageCode = toStage,

            Size = size,

            ToStationCode = toStation.Code,

            IsEmptyTray = input.IsEmptyTray,

            IsWipTray = input.IsWipTray,

            RobotCode = input.RobotCode,

            Source = input.Source,

            Status = TransferRequestStatus.Waiting,

        }, ct);

        await PublishRequestChangedAsync(request, ct);

        _logger.LogInformation(

            "Receive request {RequestId} waiting: {FromStage} -> {ToStation} ({ToStage})",

            request.Id, request.FromStageCode, toStation.Code, toStage);

        return request;



    }



    public async Task<TransferRequest> CancelAsync(Guid id, CancellationToken ct = default)
    {
        var cancelled = await _transferRequestRepository.CancelIfPendingAsync(id, ct);
        if (cancelled is null)
        {
            var existing = await _transferRequestRepository.GetByIdAsync(id, ct);
            if (existing is null)
            {
                throw new InvalidOperationException("Yêu cầu không tồn tại");
            }

            throw new InvalidOperationException("Chỉ có thể hủy yêu cầu đang chờ hoặc đang xử lý");
        }

        await PublishRequestChangedAsync(cancelled, ct);
        return cancelled;
    }



    public Task<IReadOnlyList<TransferRequest>> GetWaitingByStageAsync(string stageCode, CancellationToken ct = default) =>

        _transferRequestRepository.GetWaitingByStageAsync(stageCode, ct);



    public Task<IReadOnlyList<TransferRequest>> GetAllWaitingAsync(CancellationToken ct = default) =>

        _transferRequestRepository.GetAllWaitingAsync(ct);



    public Task<TransferRequest?> GetByIdAsync(Guid id, CancellationToken ct = default) =>

        _transferRequestRepository.GetByIdAsync(id, ct);

    public async Task ProcessDispatchCycleAsync(CancellationToken ct = default)
    {
        var processed = new HashSet<Guid>();
        var pending = await _transferRequestRepository.GetPendingWarehouseRequestsAsync(ct);
        var waiting = await _transferRequestRepository.GetAllWaitingAsync(ct);

        _logger.LogDebug(
            "Transfer dispatch cycle started: {WarehousePending} warehouse-pending, {Pending} waiting/dispatching",
            pending.Count,
            waiting.Count);

        foreach (var request in pending)
        {
            processed.Add(request.Id);
            await TryDispatchRequestAsync(request.Id, ct);
        }

        foreach (var request in waiting)
        {
            if (processed.Contains(request.Id))
            {
                continue;
            }

            await TryDispatchRequestAsync(request.Id, ct);
        }
    }

    private async Task TryDispatchRequestAsync(Guid requestId, CancellationToken ct)
    {
        try
        {
            await DispatchRequestAsync(requestId, ct);
        }
        catch (StationNotAvailableException ex)
        {
            _logger.LogDebug(
                ex,
                "Transfer dispatch skipped for request {RequestId}: {Reason}",
                requestId,
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transfer dispatch failed for request {RequestId}", requestId);
        }
    }

    private async Task DispatchRequestAsync(Guid requestId, CancellationToken ct)
    {
        var request = await _transferRequestRepository.ClaimWaitingAsync(requestId, ct);
        if (request is null)
        {
            var existing = await _transferRequestRepository.GetByIdAsync(requestId, ct);
            if (existing?.Status != TransferRequestStatus.Dispatching)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(existing.FlowTaskId))
            {
                _logger.LogDebug(
                    "Transfer dispatch skipped — request {RequestId} Dispatching with FlowTask {FlowTaskId}",
                    requestId,
                    existing.FlowTaskId);
                return;
            }

            request = existing;
            _logger.LogWarning(
                "Transfer dispatch resuming stuck Dispatching request {RequestId} (updated {UpdatedAt:O}, type={Type}, {FromStage}->{ToStage}, station={Station})",
                requestId,
                existing.UpdatedAt,
                existing.Type,
                existing.FromStageCode,
                existing.ToStageCode,
                existing.Type == TransferRequestType.Send ? existing.FromStationCode : existing.ToStationCode);
        }

        _logger.LogInformation(
            "Transfer dispatch claimed {RequestId}: {Type} {FromStage}->{ToStage} size={Size} station={Station} source={Source} whPending={WarehousePendingKind}",
            request.Id,
            request.Type,
            request.FromStageCode,
            request.ToStageCode,
            request.Size,
            request.Type == TransferRequestType.Send ? request.FromStationCode : request.ToStationCode,
            request.Source,
            request.WarehousePendingKind);

        try
        {
            _logger.LogInformation(
                "Transfer dispatch publishing state change for {RequestId} (Dispatching)",
                request.Id);
            await PublishRequestChangedAsync(request, ct);
            _logger.LogInformation(
                "Transfer dispatch published state change for {RequestId}",
                request.Id);

            if (request.WarehousePendingKind != WarehousePendingKind.None)
            {
                await DispatchPendingWarehouseRequestAsync(request, ct);
            }
            else if (request.Type == TransferRequestType.Send)
            {
                await DispatchSendRequestAsync(request, ct);
            }
            else
            {
                await DispatchReceiveRequestAsync(request, ct);
            }

            await EnsureNotLeftDispatchingAsync(request.Id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Transfer dispatch error for {RequestId} — reverting Dispatching to Waiting",
                request.Id);
            await ReturnDispatchingToWaitingAsync(request.Id, ct);
            throw;
        }
        finally
        {
            var final = await _transferRequestRepository.GetByIdAsync(requestId, ct);
            if (final?.Status == TransferRequestStatus.Dispatching)
            {
                _logger.LogWarning(
                    "Transfer dispatch completed but {RequestId} is still Dispatching (updated {UpdatedAt:O}) — investigate hang or missing status transition",
                    requestId,
                    final.UpdatedAt);
            }
        }
    }

    private async Task DispatchPendingWarehouseRequestAsync(TransferRequest request, CancellationToken ct)
    {
        if (request.WarehousePendingKind == WarehousePendingKind.Inbound)
        {
            _logger.LogInformation(
                "Transfer dispatch warehouse inbound retry for {RequestId} -> station {WarehouseStation}",
                request.Id,
                request.PendingWarehouseStationCode);
            var successStatus = request.ExpiresAt <= DateTime.UtcNow
                ? TransferRequestStatus.TimedOut
                : TransferRequestStatus.Completed;
            await TryStartWarehouseInboundAsync(request, successStatus, ct);
            return;
        }

        if (request.WarehousePendingKind == WarehousePendingKind.Outbound)
        {
            _logger.LogInformation(
                "Transfer dispatch warehouse outbound retry for {RequestId} -> station {WarehouseStation}",
                request.Id,
                request.PendingWarehouseStationCode);
            var result = await TryFulfillFromWarehouseAsync(request, ct);
            if (result is null || result.Status == TransferRequestStatus.Dispatching)
            {
                await ReturnToWaitingAsync(request, ct, "warehouse outbound not ready");
            }
        }
    }

    private async Task DispatchSendRequestAsync(TransferRequest request, CancellationToken ct)
    {
        if (await IsWarehouseInboundDestinationAsync(request.ToStageCode, request.Size, ct))
        {
            _logger.LogInformation(
                "Transfer dispatch send {RequestId}: destination {ToStage} is warehouse inbound",
                request.Id,
                request.ToStageCode);
            await TryStartWarehouseInboundAsync(request, TransferRequestStatus.Completed, ct);
            return;
        }

        var receive = await FindReceiveCounterpartForSendAsync(
            request.FromStageCode,
            request.ToStageCode,
            request.Size,
            request.IsEmptyTray,
            ct);

        if (receive is not null)
        {
            _logger.LogInformation(
                "Transfer dispatch send {RequestId}: found receive counterpart {ReceiveId} ({ReceiveFromStage}->{ReceiveToStage})",
                request.Id,
                receive.Id,
                receive.FromStageCode,
                receive.ToStageCode);

            var claimedReceive = await _transferRequestRepository.ClaimWaitingAsync(receive.Id, ct);
            if (claimedReceive is not null)
            {
                if (request.IsEmptyTray && string.IsNullOrWhiteSpace(claimedReceive.FromStageCode))
                {
                    claimedReceive.FromStageCode = request.FromStageCode;
                }

                await ExecuteMatchedTransferAsync(request, claimedReceive, request, ct);
                return;
            }

            _logger.LogInformation(
                "Transfer dispatch send {RequestId}: receive counterpart {ReceiveId} could not be claimed (race or already dispatching)",
                request.Id,
                receive.Id);
        }
        else
        {
            _logger.LogDebug(
                "Transfer dispatch send {RequestId}: no receive counterpart for {FromStage}->{ToStage} size={Size} emptyTray={IsEmptyTray}",
                request.Id,
                request.FromStageCode,
                request.ToStageCode,
                request.Size,
                request.IsEmptyTray);
        }

        if (request.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogInformation(
                "Transfer dispatch send {RequestId}: expired at {ExpiresAt:O} — trying warehouse inbound",
                request.Id,
                request.ExpiresAt);
            await TryStartWarehouseInboundAsync(request, TransferRequestStatus.TimedOut, ct);
            return;
        }

        await ReturnToWaitingAsync(request, ct, "no receive match yet");
    }

    private async Task DispatchReceiveRequestAsync(TransferRequest request, CancellationToken ct)
    {
        if (!request.IsWipTray)
        {
            var send = await FindSendCounterpartForReceiveAsync(request, ct);
            if (send is not null)
            {
                _logger.LogInformation(
                    "Transfer dispatch receive {RequestId}: found send counterpart {SendId} ({SendFromStage}->{SendToStage})",
                    request.Id,
                    send.Id,
                    send.FromStageCode,
                    send.ToStageCode);

                var claimedSend = await _transferRequestRepository.ClaimWaitingAsync(send.Id, ct);
                if (claimedSend is not null)
                {
                    if (request.IsEmptyTray)
                    {
                        request.FromStageCode = claimedSend.FromStageCode;
                    }

                    await ExecuteMatchedTransferAsync(claimedSend, request, request, ct);
                    return;
                }

                _logger.LogInformation(
                    "Transfer dispatch receive {RequestId}: send counterpart {SendId} could not be claimed (race or already dispatching)",
                    request.Id,
                    send.Id);
            }
            else
            {
                _logger.LogDebug(
                    "Transfer dispatch receive {RequestId}: no send counterpart for toStage={ToStage} size={Size} source={Source} emptyTray={IsEmptyTray}",
                    request.Id,
                    request.ToStageCode,
                    request.Size,
                    request.Source,
                    request.IsEmptyTray);
            }
        }

        var fromWarehouse = await TryFulfillFromWarehouseAsync(request, ct);
        if (fromWarehouse is null || fromWarehouse.Status == TransferRequestStatus.Dispatching)
        {
            await ReturnToWaitingAsync(request, ct, "no send match and warehouse fallback unavailable");
        }
    }

    private async Task ReturnDispatchingToWaitingAsync(Guid requestId, CancellationToken ct)
    {
        var current = await _transferRequestRepository.GetByIdAsync(requestId, ct);
        if (current?.Status == TransferRequestStatus.Dispatching)
        {
            _logger.LogWarning(
                "Transfer request {RequestId} reverting Dispatching -> Waiting after dispatch error",
                requestId);
            await ReturnToWaitingAsync(current, ct, "dispatch error recovery");
        }
    }

    private async Task EnsureNotLeftDispatchingAsync(Guid requestId, CancellationToken ct)
    {
        var current = await _transferRequestRepository.GetByIdAsync(requestId, ct);
        if (current?.Status == TransferRequestStatus.Dispatching &&
            string.IsNullOrWhiteSpace(current.FlowTaskId))
        {
            _logger.LogWarning(
                "Transfer request {RequestId} still Dispatching after dispatch — reverting to Waiting",
                requestId);
            await ReturnToWaitingAsync(current, ct, "dispatch incomplete safeguard");
        }
    }

    private async Task ReturnToWaitingAsync(TransferRequest request, CancellationToken ct, string? reason = null)
    {
        request.Status = TransferRequestStatus.Waiting;
        request.UpdatedAt = DateTime.UtcNow;
        await _transferRequestRepository.UpdateAsync(request, ct);
        await PublishRequestChangedAsync(request, ct);

        _logger.LogInformation(
            "Transfer request {RequestId} returned to Waiting{Reason}: {Type} {FromStage}->{ToStage} station={Station}",
            request.Id,
            string.IsNullOrWhiteSpace(reason) ? string.Empty : $" ({reason})",
            request.Type,
            request.FromStageCode,
            request.ToStageCode,
            request.Type == TransferRequestType.Send ? request.FromStationCode : request.ToStationCode);
    }


    private async Task ProcessPhase1PendingWarehouseRequestAsync(TransferRequest request, CancellationToken ct)

    {

        var current = await _transferRequestRepository.GetByIdAsync(request.Id, ct);

        if (current is null ||

            current.Status != TransferRequestStatus.Waiting ||

            current.WarehousePendingKind == WarehousePendingKind.None)

        {

            return;

        }



        if (current.WarehousePendingKind == WarehousePendingKind.Inbound)

        {
            var successStatus = current.ExpiresAt <= DateTime.UtcNow

                ? TransferRequestStatus.TimedOut

                : TransferRequestStatus.Completed;

            await TryStartWarehouseInboundAsync(current, successStatus, ct);

            return;

        }



        if (current.WarehousePendingKind == WarehousePendingKind.Outbound)

        {

            await TryFulfillFromWarehouseAsync(current, ct);

        }

    }



    private async Task<TransferRequest?> FindReceiveCounterpartForSendAsync(
        string fromStage, string toStage, Size size, bool isEmptyTray, CancellationToken ct)
    {
        if (isEmptyTray)
        {
            return await _transferRequestRepository.FindWaitingEmptyTrayReceiveAtStageAsync(toStage, size, ct);
        }

        var exact = await _transferRequestRepository.FindWaitingCounterpartAsync(
            TransferRequestType.Receive, fromStage, toStage, size, false, ct);
        if (exact is not null)
        {
            return exact;
        }

        return await _transferRequestRepository.FindWaitingAutoOpcReceiveAtStageAsync(toStage, size, false, ct);
    }



    private async Task<TransferRequest?> FindSendCounterpartForReceiveAsync(
        TransferRequest receive, CancellationToken ct)
    {
        TransferRequest? send;
        if (receive.IsEmptyTray)
        {
            send = await _transferRequestRepository.FindWaitingEmptyTraySendToStageAsync(
                receive.ToStageCode, receive.Size, ct);
        }
        else if (receive.Source == TransferRequestSource.AutoOpc)
        {
            send = await _transferRequestRepository.FindWaitingSendToStageAsync(
                receive.ToStageCode, receive.Size, false, ct);
        }
        else
        {
            send = await _transferRequestRepository.FindWaitingCounterpartAsync(
                TransferRequestType.Send, receive.FromStageCode, receive.ToStageCode, receive.Size, false, ct);
        }

        if (send is null && !receive.IsEmptyTray && receive.Source == TransferRequestSource.AutoOpc)
        {
            var blocking = await _transferRequestRepository.FindDispatchingSendsToStageAsync(
                receive.ToStageCode, receive.Size, false, ct);
            if (blocking.Count > 0)
            {
                _logger.LogWarning(
                    "Transfer dispatch receive {RequestId}: no Waiting send to {ToStage} size={Size}, but {Count} Dispatching send(s) may block matching: [{SendSummary}]",
                    receive.Id,
                    receive.ToStageCode,
                    receive.Size,
                    blocking.Count,
                    string.Join(", ", blocking.Select(s =>
                        $"{s.Id} ({s.FromStageCode}->{s.ToStageCode}, updated {s.UpdatedAt:O})")));
            }
        }

        return send;
    }



    private async Task<TransferRequest> ExecuteMatchedTransferAsync(

        TransferRequest sendRequest,

        TransferRequest receiveRequest,

        TransferRequest initiator,

        CancellationToken ct)

    {

        _logger.LogInformation(
            "Transfer dispatch executing match send {SendId} + receive {ReceiveId}, initiator={InitiatorId}",
            sendRequest.Id,
            receiveRequest.Id,
            initiator.Id);

        var (fromStationCode, toStationCode) = await ResolveMatchedRouteAsync(sendRequest, receiveRequest, ct);



        sendRequest.Status = TransferRequestStatus.Matched;

        sendRequest.MatchedRequestId = receiveRequest.Id;

        receiveRequest.Status = TransferRequestStatus.Matched;

        receiveRequest.MatchedRequestId = sendRequest.Id;

        sendRequest.UpdatedAt = DateTime.UtcNow;

        receiveRequest.UpdatedAt = DateTime.UtcNow;

        await _transferRequestRepository.UpdateAsync(sendRequest, ct);

        await _transferRequestRepository.UpdateAsync(receiveRequest, ct);
        await PublishRequestChangedAsync([sendRequest, receiveRequest], ct);



        FlowTask flowTask;

        try

        {

            flowTask = await _flowTaskStarter.StartStationToStationAsync(

                fromStationCode,

                toStationCode,

                sendRequest.RobotCode ?? receiveRequest.RobotCode,

                sendRequest.CranePositions,

                sendRequest.CassetteCode,

                sendRequest.StorageStageCode,

                sendRequest.Quantity,

                sendRequest.Product,

                sendRequest.Size,

                ct);

        }

        catch

        {

            await RevertToWaitingAsync(sendRequest, receiveRequest, ct);

            throw;

        }



        sendRequest.FlowTaskId = flowTask.Id;

        receiveRequest.FlowTaskId = flowTask.Id;

        sendRequest.UpdatedAt = DateTime.UtcNow;

        receiveRequest.UpdatedAt = DateTime.UtcNow;

        await _transferRequestRepository.UpdateAsync(sendRequest, ct);

        await _transferRequestRepository.UpdateAsync(receiveRequest, ct);
        await PublishRequestChangedAsync([sendRequest, receiveRequest], ct);



        _logger.LogInformation(

            "Matched send {SendId} + receive {ReceiveId} -> FlowTask {FlowTaskId}: {FromStation} -> {ToStation}",

            sendRequest.Id, receiveRequest.Id, flowTask.Id, fromStationCode, toStationCode);



        return initiator;

    }



    private async Task<TransferRequest?> TryFulfillFromWarehouseAsync(

        TransferRequest receiveRequest,

        CancellationToken ct)

    {

        if (!receiveRequest.IsEmptyTray && !_warehouseOptions.AllowsSize(receiveRequest.Size))
        {
            _logger.LogDebug(
                "Warehouse FIFO lookup skipped — size {Size} is disabled for warehouse outbound stock. RequestId={RequestId}, ToStage={ToStage}",
                receiveRequest.Size,
                receiveRequest.Id,
                receiveRequest.ToStageCode);
            return null;
        }

        var warehouseOut = await GetWarehouseOutboundStationAsync(receiveRequest.Size, ct, requireDoorReady: false);

        if (warehouseOut is null)

        {

            _logger.LogDebug(

                "Warehouse fallback skipped — no outbound door for size {Size}. RequestId={RequestId}",

                receiveRequest.Size, receiveRequest.Id);

            return null;

        }



        var position = await FindWarehouseOutboundPositionAsync(receiveRequest, ct);

        if (position is null || !position.IsValid)

        {

            if (receiveRequest.WarehousePendingKind == WarehousePendingKind.Outbound)

            {

                receiveRequest.WarehousePendingKind = WarehousePendingKind.None;

                receiveRequest.PendingWarehouseStationCode = null;

                receiveRequest.CranePositions = null;

                receiveRequest.Status = TransferRequestStatus.Waiting;

                receiveRequest.UpdatedAt = DateTime.UtcNow;

                await _transferRequestRepository.UpdateAsync(receiveRequest, ct);
                await PublishRequestChangedAsync(receiveRequest, ct);



                _logger.LogInformation(

                    "Receive request {RequestId} — no WMS stock, cleared outbound pending",

                    receiveRequest.Id);

                return receiveRequest;

            }



            _logger.LogDebug(

                "Warehouse fallback skipped — no stock. RequestId={RequestId}, IsEmptyTray={IsEmptyTray}, ToStage={ToStage}",

                receiveRequest.Id, receiveRequest.IsEmptyTray, receiveRequest.ToStageCode);

            return null;

        }



        var cranePositions = new List<CranePosition>

        {

            new(position.Row, position.Floor, position.PositionNumber)

        };

        var toStationCode = await ResolveReceiveToStationAsync(receiveRequest, ct);



        return await TryStartWarehouseOutboundAsync(

            receiveRequest, warehouseOut.Code, toStationCode, cranePositions, ct);

    }



    private Task<WarehouseOutboundPosition?> FindWarehouseOutboundPositionAsync(

        TransferRequest receiveRequest,

        CancellationToken ct) =>

        receiveRequest.IsEmptyTray

            ? _wmsService.FindEmptyTrayOutboundPositionAsync(ct)

            : _wmsService.FindFifoOutboundPositionAsync(receiveRequest.ToStageCode, ct);



    private async Task<TransferRequest> TryStartWarehouseOutboundAsync(

        TransferRequest receiveRequest,

        string warehouseOutCode,

        string toStationCode,

        List<CranePosition> cranePositions,

        CancellationToken ct)

    {

        var warehouseOut = await RequireStationAsync(warehouseOutCode, ct);
        if (await _stationService.GetWarehouseDoorNotReadyReasonAsync(warehouseOut) != null)
        {
            return await EnqueueWarehouseOutboundAsync(receiveRequest, warehouseOutCode, cranePositions, ct);
        }



        if (!await _stationService.IsStationFreeForNewTaskAsync(toStationCode))

        {

            throw new StationNotAvailableException("There is already a task in progress on this station");

        }



        if (!await _stationService.IsStationFreeForNewTaskAsync(warehouseOutCode))

        {

            return await EnqueueWarehouseOutboundAsync(receiveRequest, warehouseOutCode, cranePositions, ct);

        }



        FlowTask flowTask;

        try

        {

            flowTask = await _flowTaskStarter.StartStationToStationAsync(

                warehouseOutCode,

                toStationCode,

                receiveRequest.RobotCode,

                cranePositions,

                taskSize: receiveRequest.Size,

                ct: ct);

        }

        catch (StationNotAvailableException)

        {

            return await EnqueueWarehouseOutboundAsync(receiveRequest, warehouseOutCode, cranePositions, ct);

        }



        receiveRequest.Status = TransferRequestStatus.Completed;

        receiveRequest.FlowTaskId = flowTask.Id;

        receiveRequest.CranePositions = cranePositions;

        receiveRequest.WarehousePendingKind = WarehousePendingKind.None;

        receiveRequest.PendingWarehouseStationCode = null;

        receiveRequest.UpdatedAt = DateTime.UtcNow;

        await _transferRequestRepository.UpdateAsync(receiveRequest, ct);
        await PublishRequestChangedAsync(receiveRequest, ct);



        _logger.LogInformation(

            "Receive request {RequestId} fulfilled from warehouse — FlowTask {FlowTaskId}: {FromStation} -> {ToStation}",

            receiveRequest.Id, flowTask.Id, warehouseOutCode, toStationCode);



        return receiveRequest;

    }



    private async Task<TransferRequest> EnqueueWarehouseOutboundAsync(

        TransferRequest receiveRequest,

        string warehouseOutCode,

        List<CranePosition> cranePositions,

        CancellationToken ct)

    {

        receiveRequest.WarehousePendingKind = WarehousePendingKind.Outbound;

        receiveRequest.PendingWarehouseStationCode = warehouseOutCode;

        receiveRequest.CranePositions = cranePositions;

        receiveRequest.Status = TransferRequestStatus.Waiting;

        receiveRequest.UpdatedAt = DateTime.UtcNow;

        await _transferRequestRepository.UpdateAsync(receiveRequest, ct);
        await PublishRequestChangedAsync(receiveRequest, ct);



        _logger.LogInformation(

            "Receive request {RequestId} waiting for warehouse OUT {WarehouseCode}",

            receiveRequest.Id, warehouseOutCode);



        return receiveRequest;

    }



    private async Task ProcessSendTimeoutAsync(TransferRequest sendRequest, CancellationToken ct)

    {

        var current = await _transferRequestRepository.GetByIdAsync(sendRequest.Id, ct);

        if (current is null || current.Status != TransferRequestStatus.Waiting)

        {

            return;

        }



        await TryStartWarehouseInboundAsync(current, TransferRequestStatus.TimedOut, ct);

    }



    private async Task<TransferRequest> ExecuteSendToWarehouseAsync(

        TransferRequest sendRequest,

        CancellationToken ct)

    {

        return await TryStartWarehouseInboundAsync(sendRequest, TransferRequestStatus.Completed, ct);

    }



    private async Task<TransferRequest> KeepSendWaitingWithoutWarehouseAsync(

        TransferRequest request,

        CancellationToken ct)

    {

        var shouldUpdate =
            request.Status != TransferRequestStatus.Waiting ||
            request.WarehousePendingKind != WarehousePendingKind.None ||
            request.PendingWarehouseStationCode is not null;

        if (shouldUpdate)
        {
            request.Status = TransferRequestStatus.Waiting;

            request.WarehousePendingKind = WarehousePendingKind.None;

            request.PendingWarehouseStationCode = null;

            request.UpdatedAt = DateTime.UtcNow;

            await _transferRequestRepository.UpdateAsync(request, ct);
            await PublishRequestChangedAsync(request, ct);
        }



        _logger.LogDebug(

            "Send request {RequestId} timed out but no warehouse inbound is available for size {Size}; keeping waiting",

            request.Id, request.Size);



        return request;

    }



    private async Task<TransferRequest> TryStartWarehouseInboundAsync(

        TransferRequest request,

        TransferRequestStatus successStatus,

        CancellationToken ct)

    {

        var route = await TryResolveSendToWarehouseRouteAsync(request, ct);

        if (route is null)
        {
            return await KeepSendWaitingWithoutWarehouseAsync(request, ct);
        }

        var (fromStationCode, warehouseInCode) = route.Value;



        var warehouseIn = await RequireStationAsync(warehouseInCode, ct);
        if (await _stationService.GetWarehouseDoorNotReadyReasonAsync(warehouseIn) != null)
        {
            return await EnqueueWarehouseInboundAsync(request, warehouseInCode, ct);
        }



        if (!await _stationService.IsStationFreeForNewTaskAsync(fromStationCode))
        {
            return await EnqueueWarehouseInboundAsync(request, warehouseInCode, ct);
        }



        if (!await _stationService.IsStationFreeForNewTaskAsync(warehouseInCode))

        {

            return await EnqueueWarehouseInboundAsync(request, warehouseInCode, ct);

        }

        var emptyInboundPositions = await _wmsService.TryFindEmptyInboundPositionsAsync(ct);
        if (emptyInboundPositions is null || emptyInboundPositions.Count == 0)
        {
            _logger.LogInformation(
                "Send request {RequestId}: warehouse has no empty inbound positions — waiting for {WarehouseCode}",
                request.Id,
                warehouseInCode);
            return await EnqueueWarehouseInboundAsync(request, warehouseInCode, ct);
        }



        FlowTask flowTask;

        try

        {

            flowTask = await StartSendToWarehouseFlowTaskAsync(request, fromStationCode, warehouseInCode, ct);

        }

        catch (StationNotAvailableException)

        {

            return await EnqueueWarehouseInboundAsync(request, warehouseInCode, ct);

        }



        request.Status = successStatus;

        request.FlowTaskId = flowTask.Id;

        request.WarehousePendingKind = WarehousePendingKind.None;

        request.PendingWarehouseStationCode = null;

        request.UpdatedAt = DateTime.UtcNow;

        await _transferRequestRepository.UpdateAsync(request, ct);
        await PublishRequestChangedAsync(request, ct);



        _logger.LogInformation(

            "Send request {RequestId} fulfilled to warehouse — FlowTask {FlowTaskId}: {FromStation} -> {ToStation}",

            request.Id, flowTask.Id, fromStationCode, warehouseInCode);



        return request;

    }



    private async Task<TransferRequest> EnqueueWarehouseInboundAsync(

        TransferRequest request,

        string warehouseInCode,

        CancellationToken ct)

    {

        request.WarehousePendingKind = WarehousePendingKind.Inbound;

        request.PendingWarehouseStationCode = warehouseInCode;

        request.Status = TransferRequestStatus.Waiting;

        request.UpdatedAt = DateTime.UtcNow;

        await _transferRequestRepository.UpdateAsync(request, ct);
        await PublishRequestChangedAsync(request, ct);



        _logger.LogInformation(

            "Send request {RequestId} waiting for warehouse IN {WarehouseCode}",

            request.Id, warehouseInCode);



        return request;

    }



    private Task<FlowTask> StartSendToWarehouseFlowTaskAsync(

        TransferRequest sendRequest,

        string fromStationCode,

        string warehouseInCode,

        CancellationToken ct) =>

        _flowTaskStarter.StartStationToStationAsync(

            fromStationCode,

            warehouseInCode,

            sendRequest.RobotCode,

            [new CranePosition(0, 0, 0)],

            sendRequest.CassetteCode,

            ResolveWarehouseInboundStageCode(sendRequest),

            sendRequest.Quantity,

            sendRequest.Product,

            sendRequest.Size,

            ct);



    private async Task<bool> IsWarehouseInboundDestinationAsync(string toStage, Size size, CancellationToken ct)

    {
        if (string.IsNullOrWhiteSpace(toStage))
        {
            return false;
        }

        var stations = await _stationService.GetAllStations();

        return stations.Any(s =>
            s.HasWarehouseDoor && s.Type == InOut.IN && s.SupportsSize(size) && s.IsActive &&
            (string.Equals(s.StageCode, toStage, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(s.Code, toStage, StringComparison.OrdinalIgnoreCase)));

    }



    private async Task<(string FromStationCode, string ToStationCode)> ResolveMatchedRouteAsync(

        TransferRequest sendRequest,

        TransferRequest receiveRequest,

        CancellationToken ct)

    {

        var fromStationCode = sendRequest.FromStationCode
            ?? throw new InvalidOperationException("Send request thiếu from_station");


        var toStationCode = receiveRequest.ToStationCode
            ?? throw new InvalidOperationException("Receive request thiếu to_station");




        await ValidatePinnedStationAsync(fromStationCode, sendRequest.FromStageCode, ct);

        await ValidatePinnedStationAsync(toStationCode, receiveRequest.ToStageCode, ct);



        return (fromStationCode, toStationCode);

    }



    private static string ResolveWarehouseInboundStageCode(TransferRequest request) =>

        request.IsEmptyTray

            ? string.Empty

            : request.StorageStageCode ?? request.FromStageCode;



    private async Task<(string FromStationCode, string WarehouseInCode)?> TryResolveSendToWarehouseRouteAsync(

        TransferRequest sendRequest,

        CancellationToken ct)

    {

        var fromStationCode = sendRequest.FromStationCode
            ?? throw new InvalidOperationException("Send request thiếu from_station");




        await ValidatePinnedStationAsync(fromStationCode, sendRequest.FromStageCode, ct);



        var warehouseIn = await GetWarehouseInboundStationForToStageAsync(sendRequest.ToStageCode, sendRequest.Size, ct, requireDoorReady: false)

            ?? await GetWarehouseInboundStationAsync(sendRequest.Size, ct, requireDoorReady: false);




        if (warehouseIn is null)
        {
            return null;
        }

        return (fromStationCode, warehouseIn.Code);

    }



    private async Task<string> ResolveReceiveToStationAsync(TransferRequest receiveRequest, CancellationToken ct)

    {

        var toStationCode = receiveRequest.ToStationCode
            ?? throw new InvalidOperationException("Receive request thiếu to_station");




        await ValidatePinnedStationAsync(toStationCode, receiveRequest.ToStageCode, ct);

        return toStationCode;

    }



    private async Task<Station> RequireStationAsync(string stationCode, CancellationToken ct) =>

        await _stationService.GetStationByCode(stationCode)
            ?? throw new InvalidOperationException($"Trạm {stationCode} không tồn tại");




    private static string RequireStage(Station station, string stationCode)

    {

        if (string.IsNullOrWhiteSpace(station.StageCode))

        {

            throw new InvalidOperationException($"Trạm {stationCode} chưa gán công đoạn");

        }



        return station.StageCode;

    }



    private static string RequireFromStage(string? fromStage)

    {

        if (string.IsNullOrWhiteSpace(fromStage))

        {

            throw new InvalidOperationException("from_stage là bắt buộc khi nhận hàng");

        }



        return fromStage;

    }



    private async Task RequireDestinationStageSupportsSizeAsync(string toStage, Size size, CancellationToken ct)

    {

        var stations = await _stationService.GetAllStations();

        var hasMatchingStation = stations.Any(s =>

            s.IsActive &&

            s.SupportsSize(size) &&

            (string.Equals(s.StageCode, toStage, StringComparison.OrdinalIgnoreCase) ||

             string.Equals(s.Code, toStage, StringComparison.OrdinalIgnoreCase)));



        if (!hasMatchingStation)

        {

            throw new InvalidOperationException(

                $"Công đoạn đích {toStage} không có trạm nào hỗ trợ size {size}");

        }

    }



    private async Task RequireNoDuplicateSendAsync(

        string fromStage,
        string toStage,
        Size size,
        bool isEmptyTray,
        string fromStationCode,
        string? cassetteCode,
        CancellationToken ct)

    {

        if (await _transferRequestRepository.ExistsActiveAsync(

                TransferRequestType.Send, fromStage, toStage, size, isEmptyTray, fromStationCode, cassetteCode, ct))

        {

            throw new InvalidOperationException("Đã có yêu cầu gửi hàng đang chờ cho trạm này");

        }

    }



    private async Task RequireNoConcurrentSendOnStationAsync(string fromStationCode, CancellationToken ct)

    {

        if (await _transferRequestRepository.ExistsActiveSendOnStationAsync(fromStationCode, ct))

        {

            throw new InvalidOperationException("Đã có yêu cầu gửi hàng đang chờ tại trạm này");

        }

    }



    private async Task RequireNoConcurrentReceiveOnStationAsync(string toStationCode, CancellationToken ct)

    {

        if (await _transferRequestRepository.ExistsActiveReceiveOnStationAsync(toStationCode, ct))

        {

            throw new InvalidOperationException("Đã có yêu cầu nhận hàng đang chờ cho trạm này");

        }

    }



    private async Task ValidatePinnedStationAsync(string stationCode, string expectedStageCode, CancellationToken ct)

    {

        var station = await _stationService.GetStationByCode(stationCode)
            ?? throw new InvalidOperationException($"Trạm {stationCode} không tồn tại");




        if (!string.Equals(station.StageCode, expectedStageCode, StringComparison.OrdinalIgnoreCase))

        {

            throw new InvalidOperationException(

                $"Trạm {stationCode} không thuộc công đoạn {expectedStageCode}");

        }

    }



    private async Task RevertToWaitingAsync(

        TransferRequest sendRequest, TransferRequest receiveRequest, CancellationToken ct)

    {

        sendRequest.Status = TransferRequestStatus.Waiting;

        sendRequest.MatchedRequestId = null;

        sendRequest.FlowTaskId = null;

        receiveRequest.Status = TransferRequestStatus.Waiting;

        receiveRequest.MatchedRequestId = null;

        receiveRequest.FlowTaskId = null;

        sendRequest.UpdatedAt = DateTime.UtcNow;

        receiveRequest.UpdatedAt = DateTime.UtcNow;

        await _transferRequestRepository.UpdateAsync(sendRequest, ct);

        await _transferRequestRepository.UpdateAsync(receiveRequest, ct);
        await PublishRequestChangedAsync([sendRequest, receiveRequest], ct);

    }



    private async Task<Station?> GetWarehouseInboundStationAsync(Size size, CancellationToken ct, bool requireDoorReady = true)

    {

        var stations = await _stationService.GetAllStations();

        foreach (var station in stations.Where(s =>

            s.HasWarehouseDoor && s.Type == InOut.IN && s.SupportsSize(size) && s.IsActive))

        {

            if (!await _stationService.IsStationPlcRunningAsync(station))
                continue;

            if (requireDoorReady)
            {
                var warehouseDoorReason = await _stationService.GetWarehouseDoorNotReadyReasonAsync(station);
                if (warehouseDoorReason != null)
                {
                    _logger.LogDebug(
                        "Warehouse inbound station {StationCode} skipped: {Reason}",
                        station.Code, warehouseDoorReason);
                    continue;
                }
            }

            return station;

        }



        return null;

    }



    private async Task<Station?> GetWarehouseInboundStationForToStageAsync(string toStage, Size size, CancellationToken ct, bool requireDoorReady = true)

    {

        if (string.IsNullOrWhiteSpace(toStage))

        {

            return null;

        }



        var stations = await _stationService.GetAllStations();

        foreach (var station in stations.Where(s =>

            s.HasWarehouseDoor && s.Type == InOut.IN && s.SupportsSize(size) && s.IsActive &&

            (string.Equals(s.StageCode, toStage, StringComparison.OrdinalIgnoreCase) ||

             string.Equals(s.Code, toStage, StringComparison.OrdinalIgnoreCase))))

        {

            if (!await _stationService.IsStationPlcRunningAsync(station))
                continue;

            if (requireDoorReady)
            {
                var warehouseDoorReason = await _stationService.GetWarehouseDoorNotReadyReasonAsync(station);
                if (warehouseDoorReason != null)
                {
                    _logger.LogDebug(
                        "Warehouse inbound station {StationCode} skipped: {Reason}",
                        station.Code, warehouseDoorReason);
                    continue;
                }
            }

            return station;

        }



        return null;

    }



    private async Task<Station?> GetWarehouseOutboundStationAsync(Size size, CancellationToken ct, bool requireDoorReady = true)

    {

        var stations = await _stationService.GetAllStations();

        foreach (var station in stations.Where(s =>

            s.HasWarehouseDoor && s.Type == InOut.OUT && s.SupportsSize(size) && s.IsActive))

        {

            if (!await _stationService.IsStationPlcRunningAsync(station))
                continue;

            if (requireDoorReady)
            {
                var warehouseDoorReason = await _stationService.GetWarehouseDoorNotReadyReasonAsync(station);
                if (warehouseDoorReason != null)
                {
                    _logger.LogDebug(
                        "Warehouse outbound station {StationCode} skipped: {Reason}",
                        station.Code, warehouseDoorReason);
                    continue;
                }
            }

            return station;

        }



        return null;

    }

    private Task PublishRequestChangedAsync(TransferRequest request, CancellationToken ct) =>
        _publisher.PublishAsync(new TransferRequestChanged(request), ct);

    private Task PublishRequestChangedAsync(IEnumerable<TransferRequest> requests, CancellationToken ct) =>
        _publisher.PublishAsync(requests.Select(request => new TransferRequestChanged(request)), ct);

}



public sealed class SendTransferRequest

{

    public required string FromStation { get; init; }

    public required string ToStage { get; init; }

    public string? RobotCode { get; init; }

    public bool IsEmptyTray { get; init; }

    public string? CassetteCode { get; init; }

    public string? StorageStageCode { get; init; }

    public int? Quantity { get; init; }

    public string? Product { get; init; }

    public int? Size { get; init; }

    public TransferRequestSource Source { get; init; } = TransferRequestSource.Manual;

}



public sealed class ReceiveTransferRequest

{

    public string? FromStage { get; init; }

    public required string ToStation { get; init; }

    public string? RobotCode { get; init; }

    public bool IsEmptyTray { get; init; }

    public bool IsWipTray { get; init; }

    public int? Size { get; init; }

    public TransferRequestSource Source { get; init; } = TransferRequestSource.Manual;

}


