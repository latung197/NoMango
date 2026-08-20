using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Wcs.Api.Configs;
using Wcs.Api.OrderMatching;
using Wcs.Common.Exceptions;

namespace Wcs.Api.Workers;

public sealed class TransferRequestDispatcherWorker(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<OrderMatchingOptions> options,
    ILogger<TransferRequestDispatcherWorker> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly OrderMatchingOptions _options = options.Value;
    private readonly ILogger<TransferRequestDispatcherWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "TransferRequestDispatcherWorker started (interval={Interval}s)",
            _options.PollingIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var matchingService = scope.ServiceProvider.GetRequiredService<TransferMatchingService>();
                await matchingService.ProcessDispatchCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (StationNotAvailableException ex)
            {
                _logger.LogDebug(
                    ex,
                    "Transfer dispatch skipped because station is not available: {Reason}",
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in TransferRequestDispatcherWorker");
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

        _logger.LogInformation("TransferRequestDispatcherWorker stopping");
    }
}
