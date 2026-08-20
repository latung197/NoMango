using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;

namespace Wcs.Infrastructure.Repositories;

public class FlowTasksRepository(WcsDbContext context) : IFlowTasksRepository
{
    private readonly WcsDbContext _context = context;

    public async Task<IEnumerable<FlowTask>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Include(ft => ft.History)
            .OrderByDescending(ft => ft.CreatedAt)
            .ToListAsync(cancellationToken);

        var flowTasks = await Task.WhenAll(dbModels.Select(async dbModel =>
        {
            var fromStation = new Station
            {
                Code = dbModel.FromStationCode
            };
            var toStation = new Station
            {
                Code = dbModel.ToStationCode
            };
            return dbModel.ToDomain(null, fromStation, toStation);
        }));

        return flowTasks;
    }

    public async Task<IEnumerable<FlowTask>> GetListAsync(StageArea area, string stage, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;

        var dbModels = await _context.FlowTasks
            .Where(ft => ft.CreatedAt >= today)
            .Include(ft => ft.WaitingSets)
            .Include(ft => ft.History)
            .OrderByDescending(ft => ft.CreatedAt)
            .ToListAsync(cancellationToken);

        var flowTasks = await Task.WhenAll(dbModels.Select(async dbModel =>
        {
            var fromStation = new Station
            {
                Code = dbModel.FromStationCode
            };
            var toStation = new Station
            {
                Code = dbModel.ToStationCode
            };
            return dbModel.ToDomain(null, fromStation, toStation);
        }));

        return flowTasks;
    }

    public async Task<IEnumerable<FlowTask>> GetCancelledTasksAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.FlowTasks
            .Include(ft => ft.WaitingSets)
            .Where(ft => ft.Status == FlowStatus.Cancelled)
            .OrderByDescending(ft => ft.UpdatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return dbModels.Select(dbModel =>
        {
            var fromStation = new Station { Code = dbModel.FromStationCode };
            var toStation = new Station { Code = dbModel.ToStationCode };
            return dbModel.ToDomain(null, fromStation, toStation);
        });
    }

    public async Task<FlowTask?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.FlowTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(ft => ft.Id == id, cancellationToken);

        if (dbModel is null)
        {
            return null;
        }

        var fromStation = new Station { Code = dbModel.FromStationCode };
        var toStation = new Station { Code = dbModel.ToStationCode };
        return dbModel.ToDomain(null, fromStation, toStation);
    }
}