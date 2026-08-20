using Wcs.Common.Abstractions;
using Wcs.Api.Configs;
using Wcs.Common.Entities;
using Wcs.Common.Abstractions.Repositories;

namespace Wcs.Api.Services;

public class RobotService(IConfigService configService, ILogger<RobotService> logger, IServiceScopeFactory scopeFactory)
{
    private readonly IConfigService _configService = configService;
    private readonly ILogger<RobotService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    /// <summary>
    /// Load Robot từ config theo code
    /// </summary>
    public async Task<Robot?> GetRobotByCode(string code, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var amrRepository = scope.ServiceProvider.GetRequiredService<IAmrRepository>();
        var amr = await amrRepository.GetByCodeAsync(code, cancellationToken);
        if (amr is null) return null;
        return new Robot(amr.Code, amr.Name, amr.IsActive);
    }
}
