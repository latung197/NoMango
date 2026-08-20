using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Repositories;

public class ErrorRepository(WcsDbContext context) : IErrorRepository
{
    private readonly WcsDbContext _context = context;

    public async Task<IEnumerable<Error>> GetListAsync(ErrorType? type, string? area, CancellationToken cancellationToken = default)
    {
        IQueryable<ErrorDbModel> query = _context.Errors;
        var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);
        return await query
            .Where(e =>
                (type == null || e.ErrorType.Equals(type)) &&
                (string.IsNullOrWhiteSpace(area) || string.Equals(area, e.Area)) &&
                (e.Status != ErrorStatus.Resolved || e.CreatedAt >= twentyFourHoursAgo)
            )
            .OrderByDescending(s => s.CreatedAt)
            .ThenBy(s => s.Area)
            //.Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    /*public async Task<IEnumerable<Error>> ErrorByAreaListRequest(string? area, CancellationToken cancellationToken = default)
    {
        var errorModels = await _context.Errors
                                .Where(s => (string.IsNullOrWhiteSpace(area) || s.Area == StageArea.FromString(area)))
                                .OrderBy(s => s.Area)
                                .ThenBy(s => s.Code)
                                .Select(dbModel => dbModel.ToDomain())
                                .ToListAsync(cancellationToken);

        var flowModels = await _context.Flows
                                    .Include(f => f.Steps)
                                    .Where(s => (string.IsNullOrWhiteSpace(area) || s.Area == StageArea.FromString(area)) && s.ReturnEmpty && s.Status == FlowStatus.Active)
                                    .OrderBy(s => s.Priority)
                                    .ThenBy(s => s.Name)
                                    .ThenBy(s => s.CreatedAt)
                                    .ThenBy(s => s.UpdatedAt)
                                    .Select(dbModel => dbModel.ToDomain())
                                    .ToListAsync(cancellationToken);

        return errorModels.Select(error =>
        {
            error.ReturnEmpty = flowModels.Any(flow => flow.Steps.Any(step => step.Stage == error.Code));
            return error;
        });
    }*/

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Errors.CountAsync(cancellationToken);
    }

    public async Task<Error?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Errors.Where(e => e.Code == code).FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Error?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Errors.Where(e => e.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<IEnumerable<Error>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.Errors
            .Where(e => ids.Contains(e.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return dbModels.Select(db => db.ToDomain()).ToList();
    }

    public async Task<Error> CreateAsync(Error error, CancellationToken cancellationToken = default)
    {
        _context.Errors.Add(error.ToDbModel());
        await _context.SaveChangesAsync(cancellationToken);
        return error;
    }

    public async Task UpdateAsync(Error error, CancellationToken cancellationToken = default)
    {
        var existingError = await _context.Errors.FirstOrDefaultAsync(e => e.Id == error.Id, cancellationToken);
        if (existingError is null)
        {
            throw new InvalidOperationException($"Error với id {error.Id} không tồn tại");
        }

        existingError.Code = error.Code;
        existingError.Message = error.Message;
        existingError.ErrorType = error.ErrorType;
        //existingError.StackTrace = error.StackTrace;
        //existingError.ActionRequired = error.ActionRequired;
        //existingError.ErrorDate = error.ErrorDate;
        //existingError.ResolvedDate = error.ResolvedDate;
        //existingError.IsResolved = error.IsResolved;
        existingError.Area = error.Area?.ToString();
        //existingError.Stage = error.Stage;
        //existingError.Station = error.Station;
        existingError.UpdatedAt = error.UpdatedAt;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateBulkAsync(IEnumerable<Error> errors, CancellationToken cancellationToken = default)
    {
        var dbModels = errors.Select(db => db.ToDbModel());
        _context.Errors.UpdateRange(dbModels);
        await _context.SaveChangesAsync(cancellationToken);
    }
}