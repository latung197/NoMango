using Wcs.Common.Entities;

namespace Wcs.Common.Abstractions.Repositories;

public interface IStepRepository
{
    Task<IEnumerable<Step>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<Step?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Step?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Step> CreateAsync(Step step, CancellationToken cancellationToken = default);
    Task UpdateAsync(Step step, CancellationToken cancellationToken = default);
}
