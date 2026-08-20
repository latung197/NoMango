using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using Wcs.Api.Configs;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Events;

namespace Wcs.Api.Workers;

public class FlowTaskTimeoutWorker(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<FlowTimeoutOptions> options,
    ILogger<FlowTaskTimeoutWorker> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly FlowTimeoutOptions _options = options.Value;
    private readonly ILogger<FlowTaskTimeoutWorker> _logger = logger;
    private readonly HashSet<string> _reportedTimeouts = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FlowTaskTimeoutWorker started (enabled={Enabled}, interval={Interval}s)",
            _options.Enabled, _options.PollingIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_options.Enabled)
                {
                    await CheckTimeoutsAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking flow task timeouts");
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

        _logger.LogInformation("FlowTaskTimeoutWorker stopping");
    }

    private async Task CheckTimeoutsAsync(CancellationToken ct)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFlowTaskRepository>();
        var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        var activeTasks = await repository.GetActiveTasksAsync(ct);
        foreach (var task in activeTasks)
        {
            if (!task.CurrentStepStartedAt.HasValue)
            {
                continue;
            }

            if (!FlowStepTimeoutRules.ShouldApplyTimeout(task, _options))
            {
                continue;
            }

            var timeout = _options.GetTimeoutForStep(task.CurrentStep);
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                continue;
            }

            var elapsed = DateTime.UtcNow - task.CurrentStepStartedAt.Value;
            if (elapsed < timeout)
            {
                continue;
            }

            var cacheKey = BuildCacheKey(task);
            if (IsTimeoutAlreadyReported(cacheKey))
            {
                continue;
            }
            MarkTimeoutReported(cacheKey);

            _logger.LogWarning("Task {TaskId} timed out at step {Step}. Elapsed {ElapsedSeconds}s >= limit {TimeoutSeconds}s",
                task.Id, task.CurrentStep, elapsed.TotalSeconds, timeout.TotalSeconds);

            await publisher.PublishAsync(new TimeoutFired(task.Id.ToString(), task.CurrentStep), ct);
        }
    }

    private static string BuildCacheKey(FlowTask task)
    {
        var startedAtTicks = task.CurrentStepStartedAt?.Ticks ?? 0;
        return $"{task.Id:N}:{(int)task.CurrentStep}:{startedAtTicks}";
    }

    private bool IsTimeoutAlreadyReported(string key)
    {
        lock (_reportedTimeouts)
        {
            return _reportedTimeouts.Contains(key);
        }
    }

    private void MarkTimeoutReported(string key)
    {
        lock (_reportedTimeouts)
        {
            _reportedTimeouts.Add(key);
        }
    }
}

