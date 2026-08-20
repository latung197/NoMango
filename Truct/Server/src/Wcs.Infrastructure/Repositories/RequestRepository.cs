using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;

namespace Wcs.Infrastructure.Repositories;

public class RequestRepository(WcsDbContext context, ILogger<RequestRepository> logger) : IRequestRepository
{
    private readonly WcsDbContext _context = context;
    private readonly ILogger<RequestRepository> _logger = logger;

    public async Task<IEnumerable<Request>> GetAllAsync(string? stage, CancellationToken cancellationToken = default)
    {
        //if (string.IsNullOrWhiteSpace(stage))
        var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);
        var dbModels = await _context.Requests
            //.Where(r => r.IsActive && (r.CurrentStep < RequestStep.Completed || r.CreatedAt >= twentyFourHoursAgo))
            //.Where(r => string.IsNullOrWhiteSpace(stage) || r.StageCode.Equals(stage))
            .Where(r => r.IsActive &&
                        (r.CurrentStep < RequestStep.Completed || r.CreatedAt >= twentyFourHoursAgo) &&
                        (string.IsNullOrWhiteSpace(stage) || r.StageCode.Equals(stage))
            )
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            //.OrderBy(r => r.Code).ThenByDescending(r => r.UpdatedAt)
            .OrderBy(r => r.CreatedAt)
            //.Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);

        //var stageModel = await _stageService.GetStationByCode(dbModel.FromStationCode);
        var stageModels = await _context.Stages
                            .Where(s => s.IsActive)
                            .Select(dbModel => dbModel.ToDomain())
                            .ToListAsync(cancellationToken);

        var stationModels = await _context.Stations
                            .Where(s => s.IsActive)
                            .Include(s => s.Tags)
                            .Select(dbModel => dbModel.ToDomain())
                            .ToListAsync(cancellationToken);

        var flowTaskModels = await _context.FlowTasks
                            //.Where(ft => ft.CreatedAt >= today)
                            //.Include(ft => ft.WaitingSets)
                            //.Include(ft => ft.History)
                            .ToListAsync(cancellationToken);

        var flowTasks = await Task.WhenAll(flowTaskModels.Select(async m =>
        {
            var fromStation = new Station
            {
                Code = m.FromStationCode
            };
            var toStation = new Station
            {
                Code = m.ToStationCode
            };
            return m.ToDomain(null, fromStation, toStation);
        }));

        var requests = await Task.WhenAll(dbModels.Select(async dbModel =>
        {
            var stage = stageModels.FirstOrDefault(s => string.Equals(s.Code, dbModel.StageCode, StringComparison.OrdinalIgnoreCase));
            var station = stationModels.Where(s => string.Equals(s.StageCode, dbModel.StageCode, StringComparison.OrdinalIgnoreCase)).ToArray();
            var flowTask = flowTasks.FirstOrDefault(ft => ft.Id == dbModel.FlowTaskId);
            return dbModel.ToDomain(stage, station, flowTask);
        }));

        return requests;
    }

    /*public async Task<IEnumerable<Request>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Where(r => r.IsActive)
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .OrderByDescending(r => r.CreatedAt)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }*/

    /*public async Task<IEnumerable<Request>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Where(r => r.IsActive)
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .OrderByDescending(r => r.CreatedAt)
            .Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }*/

    /*public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Requests
            .Where(r => r.IsActive)
            .CountAsync(cancellationToken);
    }*/

    public async Task<Request?> GetByDeliveryAsync(string deliveryCode, int deliveryOrder, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Requests
            .Where(r => r.DeliveryCode == deliveryCode && r.DeliveryOrder > deliveryOrder)
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .AsNoTracking()
            .OrderBy(r => r.DeliveryOrder)
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Request?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Requests
            .Where(r => r.Code == code)
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Request?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Requests
            .Where(r => r.Code == code && r.IsActive)
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Requests
            .Where(r => r.Id == id)
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<Request?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Requests
            .Where(r => r.Id == id && r.IsActive)
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<IEnumerable<Request>> GetRequestsByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.Requests
            .Where(r => ids.Contains(r.Id))
            .Include(r => r.MaterialRequests)
                .ThenInclude(mr => mr.Material)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return dbModels.Select(db => db.ToDomain()).ToList();
    }

    public async Task<Request> CreateAsync(Request request, CancellationToken cancellationToken = default)
    {
        var dbModel = request.ToDbModel();
        _context.Requests.Add(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModel.ToDomain();
    }

    public async Task<IEnumerable<MaterialRequest>> CreateBulkAsync(IEnumerable<MaterialRequest> requests, CancellationToken cancellationToken = default)
    {
        var dbModels = requests.Select(r => r.ToDbModel()).ToList();
        _context.MaterialRequest.AddRange(dbModels);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModels.Select(db => db.ToDomain());
    }

    public async Task<MaterialRequest?> GetMaterialRequestByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.MaterialRequest
            .Include(mr => mr.Material)
            .AsNoTracking()
            .FirstOrDefaultAsync(mr => mr.Id == id && mr.IsActive, cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<bool> HasMaterialInRequestAsync(Guid requestId, Guid materialId, Guid? excludeMaterialRequestId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.MaterialRequest
            .Where(mr => mr.RequestId == requestId && mr.MaterialId == materialId && mr.IsActive);

        if (excludeMaterialRequestId.HasValue)
        {
            query = query.Where(mr => mr.Id != excludeMaterialRequestId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<MaterialRequest> CreateMaterialRequestAsync(MaterialRequest materialRequest, CancellationToken cancellationToken = default)
    {
        var dbModel = materialRequest.ToDbModel();
        _context.MaterialRequest.Add(dbModel);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.MaterialRequest
            .Include(mr => mr.Material)
            .AsNoTracking()
            .FirstAsync(mr => mr.Id == dbModel.Id, cancellationToken);

        return created.ToDomain();
    }

    public async Task UpdateMaterialRequestAsync(MaterialRequest materialRequest, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.MaterialRequest
            .FirstOrDefaultAsync(mr => mr.Id == materialRequest.Id, cancellationToken)
            ?? throw new InvalidOperationException($"MaterialRequest với id {materialRequest.Id} không tồn tại");

        dbModel.Quantity = materialRequest.Quantity;
        dbModel.Note = materialRequest.Note;
        dbModel.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMaterialRequestAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.MaterialRequest
            .FirstOrDefaultAsync(mr => mr.Id == id, cancellationToken);

        if (dbModel != null)
        {
            _context.MaterialRequest.Remove(dbModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateAsync(Request request, CancellationToken cancellationToken = default)
    {
        var dbModel = request.ToDbModel();
        _context.Requests.Update(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateBulkAsync(IEnumerable<Request> requests, CancellationToken cancellationToken = default)
    {
        var dbModels = requests.Select(db => db.ToDbModel());
        _context.Requests.UpdateRange(dbModels);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Requests
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (dbModel != null)
        {
            _context.Requests.Remove(dbModel);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Requests
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (dbModel != null)
        {
            dbModel.IsActive = false;
            dbModel.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}


