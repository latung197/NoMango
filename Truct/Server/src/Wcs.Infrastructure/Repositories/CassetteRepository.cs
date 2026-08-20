using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;

namespace Wcs.Infrastructure.Repositories;

public class CassetteRepository(WcsDbContext context, ILogger<CassetteRepository> logger) : ICassetteRepository
{
    private readonly WcsDbContext _context = context;
    private readonly ILogger<CassetteRepository> _logger = logger;

    public async Task<IEnumerable<Cassette>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cassettes
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cassette>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cassettes
            .Where(c => c.IsActive)
            .OrderBy(c => c.Code)
            .ThenBy(c => c.Name)
            .ThenBy(c => c.Size)
            .ThenBy(c => c.Capacity)
            .ThenBy(c => c.CreatedAt)
            .ThenBy(c => c.UpdatedAt)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cassette>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Cassettes
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cassettes
            .Where(c => c.IsActive)
            .CountAsync(cancellationToken);
    }

    public async Task<Cassette?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Cassettes
            .Where(c => c.Code == code)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Cassette?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Cassettes
            .Where(c => c.Code == code && c.IsActive)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Cassette?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Cassettes
            .Where(c => c.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Cassette?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Cassettes
            .Where(c => c.Id == id && c.IsActive)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Cassette> CreateAsync(Cassette cassette, CancellationToken cancellationToken = default)
    {
        var dbModel = cassette.ToDbModel();
        _context.Cassettes.Add(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModel.ToDomain();
    }

    public async Task UpdateAsync(Cassette cassette, CancellationToken cancellationToken = default)
    {
        var dbModel = cassette.ToDbModel();
        _context.Cassettes.Update(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Cassettes
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (dbModel != null)
        {
            _context.Cassettes.Remove(dbModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Cassettes
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (dbModel != null)
        {
            dbModel.IsActive = false;
            dbModel.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}


