using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Extensions;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Repositories;

public class StationRepository(WcsDbContext context, ILogger<StationRepository> logger) : IStationRepository
{
    private readonly WcsDbContext _context = context;
    private readonly ILogger<StationRepository> _logger = logger;

    private static IQueryable<StationDbModel> FilterBySize(IQueryable<StationDbModel> query, Size sizeEnum)
    {
        var token = $",{(int)sizeEnum},";
        return query.Where(s => ("," + s.Sizes + ",").Contains(token));
    }

    public async Task<IEnumerable<Station>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stations
            .Where(s => s.IsActive) // Chỉ lấy station đang active
            .Include(s => s.Tags)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Station>> GetListAsync(int page, int pageSize, string? code = null, string? stageCode = null, string? size = null, CancellationToken cancellationToken = default)
    {
        IQueryable<StationDbModel> query = _context.Stations
                                                   .Where(s => s.IsActive)
                                                   .Include(s => s.Tags);

        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(s => s.Code.Contains(code));

        if (!string.IsNullOrWhiteSpace(stageCode))
            query = query.Where(s => s.StageCode.Contains(stageCode));

        if (!string.IsNullOrWhiteSpace(size) && Enum.TryParse<Size>(size, true, out var sizeEnum))
            query = FilterBySize(query, sizeEnum);

        return await query
            .OrderBy(s => s.StageCode)
            .ThenBy(s => s.Code)
            .Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? code = null, string? stageCode = null, string? size = null, CancellationToken cancellationToken = default)
    {
        IQueryable<StationDbModel> query = _context.Stations.Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(code))
            query = query.Where(s => s.Code.Contains(code));

        if (!string.IsNullOrWhiteSpace(stageCode))
            query = query.Where(s => s.StageCode.Contains(stageCode));

        if (!string.IsNullOrWhiteSpace(size) && Enum.TryParse<Size>(size, true, out var sizeEnum))
            query = FilterBySize(query, sizeEnum);

        return await query.CountAsync(cancellationToken);
    }
    
    public async Task<Station?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Stations
            .Where(s => s.Code == code)
            .Where(s => s.IsActive)
            .Include(s => s.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<IEnumerable<Station>> GetByStageAndSizeAsync(string stageCode, string size, CancellationToken cancellationToken = default)
    {
        var sizeEnum = (Size)Enum.Parse(typeof(Size), size);
        var dbModels = await FilterBySize(
                _context.Stations
                    .Where(s => s.StageCode == stageCode)
                    .Where(s => s.IsActive),
                sizeEnum)
            .Include(s => s.Tags)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return dbModels.Select(dbModel => dbModel.ToDomain());
    }

    public async Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Stations.
            Where(s => s.Id == id)
            .Where(s => s.IsActive)
            .Include(s => s.Tags)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Station> CreateAsync(Station station, CancellationToken cancellationToken = default)
    {
        var dbModel = station.ToDbModel();
        _context.Stations.Add(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModel.ToDomain();
    }

    public async Task UpdateAsync(Station station, CancellationToken cancellationToken = default)
    {
        // Xóa waiting sets cũ
        var oldTags = await _context.StationTags
            .Where(t => t.StationId == station.Id)
            .ToListAsync(cancellationToken);
        _context.StationTags.RemoveRange(oldTags);

        var dbModel = station.ToDbModel();
        _context.Stations.Update(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Station?> FindByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.StationTags
            .Where(t => t.Tag == tag)
            .Include(t => t.Station)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.Station?.ToDomain() ?? null;
    }

    public async Task<IEnumerable<Station>> GetAllByStageAsync(string stageCode, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.Stations
            .Where(s => s.StageCode == stageCode)
            .Include(s => s.Tags)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return dbModels.Select(dbModel => dbModel.ToDomain());
    }

    public async Task<IEnumerable<Station>> GetWarehouseInboundStationsAsync(CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.Stations
            .Where(s => s.IsActive)
            .Where(s => s.HasWarehouseDoor)
            .Where(s => s.Type == InOut.IN)
            .Include(s => s.Tags)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return dbModels.Select(dbModel => dbModel.ToDomain());
    }

    public async Task DeleteAsync(Station station, CancellationToken cancellationToken = default)
    {
        // Xóa tags liên quan trước
        var tags = await _context.StationTags
            .Where(t => t.StationId == station.Id)
            .ToListAsync(cancellationToken);
        _context.StationTags.RemoveRange(tags);

        // Xóa station
        var dbModel = await _context.Stations.FindAsync([station.Id], cancellationToken);
        if (dbModel != null)
        {
            _context.Stations.Remove(dbModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}