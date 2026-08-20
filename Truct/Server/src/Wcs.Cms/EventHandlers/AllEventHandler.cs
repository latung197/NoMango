using DotNetCore.CAP;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.ValueObjects;
using Wcs.Okamura.Events;

namespace Wcs.Cms.EventHandlers;

/// <summary>
/// Handler xử lý các domain events và trigger MainFlow
/// Sử dụng IServiceScopeFactory để tạo scope mới cho mỗi event vì MainFlow là scoped service
/// </summary>
public class AllEventHandler(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<AllEventHandler> logger,
    IHubContext<NotificationsHub> hub) : ICapSubscribe
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<AllEventHandler> _logger = logger;
    private readonly IHubContext<NotificationsHub> _hub = hub;

    [CapSubscribe("Wcs.Common.Events.StationSnapshotChanged")]
    public async Task HandleStationSnapshotChangedAsync(StationSnapshotChanged @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Push SignalR StationSnapshot: {StationCode} display={DisplayState} sensor={SensorState}",
            @event.Snapshot.StationCode,
            @event.Snapshot.DisplayState,
            @event.Snapshot.SensorState);
        await _hub.Clients.All.SendAsync("StationSnapshot", @event.Snapshot, cancellationToken: cancellationToken);
    }

    [CapSubscribe("Wcs.Common.Events.StationChanged")]
    public async Task HandleStationChangedEventAsync(StationChanged @event, CancellationToken cancellationToken = default)
    {
        //await _hub.Clients.All.SendAsync(@event.GetType().Name, @event, cancellationToken: cancellationToken);
        await _hub.Clients.All.SendAsync("StationChanged", @event.Station);

        /*NoticeCreateRequest request = new()
        {
            Title = $"Station Changed: {@event.Station.Id}",
            Content = $"Station Changed Id: {@event.Station.Code}, Stage Code: {@event.Station.StageCode}"
        };
        await _hub.Clients.All.SendAsync("Notication", request);*/
    }

    [CapSubscribe("Wcs.Common.Events.FlowChangeStep")]
    public async Task HandleFlowChangeStepAsync(FlowChangeStep @event, CancellationToken cancellationToken = default)
    {
        await _hub.Clients.All.SendAsync(@event.GetType().Name, @event, cancellationToken: cancellationToken);

        /*NoticeCreateRequest request = new()
        {
            Title = $"Flow Change Step: {@event.Step}",
            Content = $"Flow Change Step TaskId: {@event.TaskId}, Step: {@event.Step}, Robot: {@event.Robot.Code}, FromStation: {@event.FromStation?.Code}, ToStation: {@event.ToStation?.Code}"
        };
        await _hub.Clients.All.SendAsync("Notication", request);*/
    }

    [CapSubscribe("Wcs.Okamura.Events.CraneInboundReadyEvent")]
    public async Task HandleCraneInboundReadyAsync(CraneInboundReadyEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("CraneInboundReady nhận được, push SignalR");
        await _hub.Clients.All.SendAsync(NotificationsHub.CraneInboundReady, @event, cancellationToken: cancellationToken);
    }

    [CapSubscribe("Wcs.Okamura.Events.CraneOutboundCompleteEvent")]
    public async Task HandleCraneOutboundCompleteAsync(CraneOutboundCompleteEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("CraneOutboundComplete nhận được, push SignalR");
        await _hub.Clients.All.SendAsync(NotificationsHub.CraneOutboundComplete, @event, cancellationToken: cancellationToken);
    }

    [CapSubscribe("Wcs.Common.Events.BizError")]
    public async Task HandleBizErrorAsync(BizError @event, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("BizError nhận được: TaskId={TaskId}, Code={Code}, Message={Message}",
            @event.TaskId, @event.Code, @event.Message);

        using var scope = _serviceScopeFactory.CreateScope();
        var errorRepository = scope.ServiceProvider.GetRequiredService<IErrorRepository>();
        var flowTasksRepository = scope.ServiceProvider.GetRequiredService<IFlowTasksRepository>();

        string? flowTaskId = null;
        string? fromStation = null;
        string? toStation = null;

        if (!string.IsNullOrWhiteSpace(@event.TaskId))
        {
            flowTaskId = @event.TaskId;
            var flowTask = await flowTasksRepository.GetByIdAsync(@event.TaskId, cancellationToken);
            if (flowTask is not null)
            {
                fromStation = flowTask.FromStation.Code;
                toStation = flowTask.ToStation.Code;
            }
        }

        var error = new Error(Guid.NewGuid(), @event.Code)
        {
            Message = @event.Message,
            ErrorType = ErrorType.Critical,
            Status = ErrorStatus.New,
            FlowTaskId = flowTaskId,
            FromStation = fromStation,
            ToStation = toStation,
        };

        await errorRepository.CreateAsync(error, cancellationToken);

        _logger.LogInformation(
            "Đã lưu BizError vào DB: Code={Code}, FlowTaskId={FlowTaskId}, FromStation={FromStation}, ToStation={ToStation}",
            @event.Code, flowTaskId, fromStation, toStation);

        var notification = @event with { FromStation = fromStation, ToStation = toStation };
        await _hub.Clients.All.SendAsync("BizError", notification, cancellationToken: cancellationToken);
    }

    [CapSubscribe("Wcs.Common.Events.TransferRequestChanged")]
    public async Task HandleTransferRequestChangedAsync(TransferRequestChanged @event, CancellationToken cancellationToken = default)
    {
        var request = @event.Request;
        var payload = new
        {
            id = request.Id,
            type = request.Type.ToString(),
            fromStageCode = request.FromStageCode,
            toStageCode = request.ToStageCode,
            size = request.Size.ToString(),
            fromStationCode = request.FromStationCode,
            toStationCode = request.ToStationCode,
            isEmptyTray = request.IsEmptyTray,
            isWipTray = request.IsWipTray,
            source = request.Source.ToString(),
            warehousePendingKind = request.WarehousePendingKind.ToString(),
            pendingWarehouseStationCode = request.PendingWarehouseStationCode,
            status = request.Status.ToString(),
            flowTaskId = request.FlowTaskId,
            expiresAt = request.ExpiresAt,
            createdAt = request.CreatedAt,
            updatedAt = request.UpdatedAt,
        };

        await _hub.Clients.All.SendAsync("TransferRequestChanged", payload, cancellationToken: cancellationToken);
    }
}
