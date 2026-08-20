using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions.Repositories;

public interface IFlowTasksRepository
{
    Task<IEnumerable<FlowTask>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetListAsync(StageArea area, string stage, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlowTask>> GetCancelledTasksAsync(int limit = 50, CancellationToken cancellationToken = default);
    Task<FlowTask?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
