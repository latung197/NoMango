using Wcs.Api.Services;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;

namespace Wcs.Api.Workers;

public sealed class StationControlCleanupWorker(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<StationControlCleanupWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<StationControlCleanupWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "StationControlCleanupWorker started (interval={IntervalSeconds}s)",
            PollingInterval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOrphanControlTagsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while cleaning orphan station control tags");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("StationControlCleanupWorker stopping");
    }

    private async Task CleanupOrphanControlTagsAsync(CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var stationRepository = scope.ServiceProvider.GetRequiredService<IStationRepository>();
        var flowTaskRepository = scope.ServiceProvider.GetRequiredService<IFlowTaskRepository>();
        var cleanup = scope.ServiceProvider.GetRequiredService<StationControlCleanupService>();

        var stations = await stationRepository.GetAllAsync(ct);
        var activeTasks = await flowTaskRepository.GetActiveTasksAsync(ct);
        var pendingTasks = await flowTaskRepository.GetPendingStartTasksAsync(ct);
        var inUseStationCodes = BuildInUseStationSet(activeTasks.Concat(pendingTasks));

        foreach (var station in stations)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(station.Tag_Control))
            {
                continue;
            }

            if (inUseStationCodes.Contains(station.Code))
            {
                continue;
            }

            await cleanup.ResetStationControlIfNonIdleAsync(
                station,
                "No active or pending flow task uses this station",
                ct);
        }
    }

    private static HashSet<string> BuildInUseStationSet(IEnumerable<FlowTask> tasks)
    {
        var stationCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var task in tasks)
        {
            AddIfNotEmpty(stationCodes, task.FromStation?.Code);
            AddIfNotEmpty(stationCodes, task.ToStation?.Code);
        }

        return stationCodes;
    }

    private static void AddIfNotEmpty(HashSet<string> stationCodes, string? stationCode)
    {
        if (!string.IsNullOrWhiteSpace(stationCode))
        {
            stationCodes.Add(stationCode);
        }
    }
}
