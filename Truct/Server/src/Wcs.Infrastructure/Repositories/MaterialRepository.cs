using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;

namespace Wcs.Infrastructure.Repositories;

public class MaterialRepository(WcsDbContext context, ILogger<MaterialRepository> logger) : IMaterialRepository
{
    private readonly WcsDbContext _context = context;
    private readonly ILogger<MaterialRepository> _logger = logger;

    public async Task<IEnumerable<Material>> GetAllAsync(string? name, string? code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(code))
        {
            return await _context.Materials
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .ThenBy(m => m.Code)
                .ThenByDescending(m => m.UpdatedAt)
                .Select(dbModel => dbModel.ToDomain())
                .ToListAsync(cancellationToken);
        }
        else
        {
            return await _context.Materials
                .Where(m => 
                    (string.IsNullOrWhiteSpace(name) || m.Name.Contains(name)) &&
                    (string.IsNullOrWhiteSpace(code) || m.Code.Contains(code)) &&
                    m.IsActive
                )
                .OrderBy(m => m.Name)
                .ThenBy(m => m.Code)
                .ThenByDescending(m => m.UpdatedAt)
                .Select(dbModel => dbModel.ToDomain())
                .ToListAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Material>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Materials
            .Where(m => m.IsActive)
            .OrderByDescending(m => m.CreatedAt)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Material>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Materials
            .Where(m => m.IsActive)
            .OrderByDescending(m => m.CreatedAt)
            .Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Materials
            .Where(m => m.IsActive)
            .CountAsync(cancellationToken);
    }

    public async Task<Material?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Materials
            .Where(m => m.Code == code)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Material?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Materials
            .Where(m => m.Code == code && m.IsActive)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Material?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Materials
            .Where(m => m.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Material?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Materials
            .Where(m => m.Id == id && m.IsActive)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Material> CreateAsync(Material material, CancellationToken cancellationToken = default)
    {
        var dbModel = material.ToDbModel();
        _context.Materials.Add(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModel.ToDomain();
    }

    public async Task UpdateAsync(Material material, CancellationToken cancellationToken = default)
    {
        var dbModel = material.ToDbModel();
        _context.Materials.Update(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Materials
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (dbModel != null)
        {
            _context.Materials.Remove(dbModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Materials
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (dbModel != null)
        {
            dbModel.IsActive = false;
            dbModel.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}


