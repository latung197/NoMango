using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions.Repositories;

public interface IStageRepository
{
    //Task<IEnumerable<Stage>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Stage>> GetListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Stage>> StageByAreaListRequest(string? area, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Stage?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Stage?> GetByCodeAndAreaAsync(string code, StageArea area, CancellationToken cancellationToken = default);
    Task<Stage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Stage> CreateAsync(Stage stage, CancellationToken cancellationToken = default);
    Task UpdateAsync(Stage stage, CancellationToken cancellationToken = default);
}

