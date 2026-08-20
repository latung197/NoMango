using Wcs.Common.Abstractions.Repositories;

namespace Wcs.Api.Services;

public interface ICraneTaskNoGenerator
{
    /// <summary>Sinh TaskNo trong khoảng 1..60000 cho D3016/D3056 (sequence lưu DB).</summary>
    Task<int> NextAsync(CancellationToken cancellationToken = default);
}

public sealed class CraneTaskNoGenerator(
    IServiceScopeFactory scopeFactory,
    ILogger<CraneTaskNoGenerator> logger) : ICraneTaskNoGenerator
{
    public const int MinValue = 1;
    public const int MaxValue = 60000;

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<CraneTaskNoGenerator> _logger = logger;

    public async Task<int> NextAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var sequenceRepository = scope.ServiceProvider
            .GetRequiredService<ICraneTaskNoSequenceRepository>();

        var taskNo = await sequenceRepository.GetNextAsync(MinValue, MaxValue, cancellationToken);
        _logger.LogDebug("Allocated crane TaskNo={TaskNo} from DB sequence", taskNo);
        return taskNo;
    }
}
