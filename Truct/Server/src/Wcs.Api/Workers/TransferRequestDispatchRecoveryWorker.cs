using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Wcs.Api.Configs;
using Wcs.Api.Services;

namespace Wcs.Api.Workers;

public sealed class TransferRequestDispatchRecoveryWorker(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<OrderMatchingOptions> options,
    ILogger<TransferRequestDispatchRecoveryWorker> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly OrderMatchingOptions _options = options.Value;
    private readonly ILogger<TransferRequestDispatchRecoveryWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "TransferRequestDispatchRecoveryWorker started (enabled={Enabled}, timeout={TimeoutMinutes}m, interval={Interval}s)",
            _options.DispatchingRecoveryEnabled,
            _options.DispatchingStuckTimeoutMinutes,
            _options.DispatchingRecoveryPollingIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_options.DispatchingRecoveryEnabled)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var recoveryService = scope.ServiceProvider
                        .GetRequiredService<TransferRequestDispatchRecoveryService>();
                    await recoveryService.RecoverStuckDispatchingAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in TransferRequestDispatchRecoveryWorker");
            }

            try
            {
                await Task.Delay(_options.GetDispatchingRecoveryPollingInterval(), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("TransferRequestDispatchRecoveryWorker stopping");
    }
}
