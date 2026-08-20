using Wcs.Common.Entities;

namespace Wcs.Common.Abstractions.Repositories;

public interface IStationRepository
{
    Task<IEnumerable<Station>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Station>> GetListAsync(int page, int pageSize, string? code = null, string? stageCode = null, string? size = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? code = null, string? stageCode = null, string? size = null, CancellationToken cancellationToken = default);
    Task<Station?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Station>> GetByStageAndSizeAsync(string code, string size, CancellationToken cancellationToken = default);
    Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Station> CreateAsync(Station station, CancellationToken cancellationToken = default);
    Task UpdateAsync(Station station, CancellationToken cancellationToken = default);
    Task<Station?> FindByTagAsync(string tag, CancellationToken cancellationToken = default);
    Task<IEnumerable<Station>> GetAllByStageAsync(string stageCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Station>> GetWarehouseInboundStationsAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Station station, CancellationToken cancellationToken = default);
}

