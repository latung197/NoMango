using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Events;

namespace Wcs.Api.EventHandlers;

public class StationSnapshotPublisher(
    IServiceScopeFactory serviceScopeFactory,
    IEventPublisher eventPublisher,
    ILogger<StationSnapshotPublisher> logger) : ICapSubscribe
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly ILogger<StationSnapshotPublisher> _logger = logger;

    public async Task PublishForOpcTagChangedAsync(OpcTagChangedEvent @event, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(@event.NodeId))
            return;

        using var scope = _serviceScopeFactory.CreateScope();
        var stationService = scope.ServiceProvider.GetRequiredService<StationService>();
        var snapshotService = scope.ServiceProvider.GetRequiredService<StationSnapshotService>();

        var station = await stationService.FindStationByTag(@event.NodeId);
        if (station == null || !snapshotService.IsConfiguredStationTag(station, @event.NodeId))
            return;

        await PublishSnapshotAsync(snapshotService, station.Code, cancellationToken);
    }

    [CapSubscribe("Wcs.Common.Events.FlowChangeStep")]
    public async Task HandleFlowChangeStepAsync(FlowChangeStep @event, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var snapshotService = scope.ServiceProvider.GetRequiredService<StationSnapshotService>();

        if (!string.IsNullOrEmpty(@event.FromStation?.Code))
            await PublishSnapshotAsync(snapshotService, @event.FromStation.Code, cancellationToken);

        if (!string.IsNullOrEmpty(@event.ToStation?.Code)
            && !string.Equals(@event.ToStation.Code, @event.FromStation?.Code, StringComparison.Ordinal))
        {
            await PublishSnapshotAsync(snapshotService, @event.ToStation.Code, cancellationToken);
        }
    }

    private async Task PublishSnapshotAsync(
        StationSnapshotService snapshotService,
        string stationCode,
        CancellationToken cancellationToken)
    {
        var snapshot = await snapshotService.BuildAsync(stationCode, cancellationToken);
        if (snapshot == null)
            return;

        _logger.LogInformation("Publishing StationSnapshotChanged for {StationCode}", stationCode);
        await _eventPublisher.PublishAsync(new StationSnapshotChanged(snapshot), cancellationToken);
    }
}
