using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions.Repositories;

public interface IFlowRepository
{
    Task<IEnumerable<Flow>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Flow>> GetListAsync(StageArea area, string stage, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Flow?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Flow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Flow> CreateAsync(Flow flow, CancellationToken cancellationToken = default);
    Task UpdateAsync(Flow flow, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
