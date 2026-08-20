using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions.Repositories;

public interface IErrorRepository
{
    Task<IEnumerable<Error>> GetListAsync(ErrorType? type, string? area, CancellationToken cancellationToken = default);
    //Task<IEnumerable<Error>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    //Task<IEnumerable<Error>> ErrorByAreaListRequest(string? area, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    //Task<Stage?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Error?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Error>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    Task<Error> CreateAsync(Error error, CancellationToken cancellationToken = default);
    Task UpdateAsync(Error error, CancellationToken cancellationToken = default);
    Task UpdateBulkAsync(IEnumerable<Error> request, CancellationToken cancellationToken = default);
}
