using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Wcs.Infrastructure.Data.Mappers;
using Wcs.Infrastructure.Data.Models;
using Wcs.Common.ValueObjects;

namespace Wcs.Infrastructure.Repositories;

public class StageRepository(WcsDbContext context) : IStageRepository
{
    private readonly WcsDbContext _context = context;

    public async Task<IEnumerable<Stage>> GetListAsync(CancellationToken cancellationToken = default)
    {
        IQueryable<StageDbModel> query = _context.Stages
            .Where(s => s.IsActive); // Chỉ lấy stage đang active
        return await query
            .OrderBy(s => s.Area)
            .ThenBy(s => s.Code)
            //.Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Stage>> StageByAreaListRequest(string? area, CancellationToken cancellationToken = default)
    {
        var stageModels = await _context.Stages
                                .Where(s => s.IsActive && (string.IsNullOrWhiteSpace(area) || s.Area == StageArea.FromString(area)))
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

        return stageModels.Select(stage =>
        {
            stage.ReturnEmpty = flowModels.Any(flow => flow.Steps.Any(step => step.Stage == stage.Code));
            return stage;
        });
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stages.Where(s => s.IsActive).CountAsync(cancellationToken);
    }

    public async Task<Stage?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Stages.Where(s => s.Code == code && s.IsActive).FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Stage?> GetByCodeAndAreaAsync(string code, StageArea area, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Stages
            .Where(s => s.Code == code && s.Area == area && s.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Stage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Stages.Where(s => s.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Stage> CreateAsync(Stage stage, CancellationToken cancellationToken = default)
    {
        _context.Stages.Add(stage.ToDbModel());
        await _context.SaveChangesAsync(cancellationToken);
        return stage;
    }

    public async Task UpdateAsync(Stage stage, CancellationToken cancellationToken = default)
    {
        var existingStage = await _context.Stages.FirstOrDefaultAsync(s => s.Id == stage.Id, cancellationToken);
        if (existingStage is null)
        {
            throw new InvalidOperationException($"Stage với id {stage.Id} không tồn tại");
        }

        existingStage.Name = stage.Name;
        existingStage.Code = stage.Code;
        existingStage.Area = stage.Area;
        existingStage.IsActive = stage.IsActive;
        existingStage.UpdatedAt = stage.UpdatedAt;
        await _context.SaveChangesAsync(cancellationToken);
    }
}