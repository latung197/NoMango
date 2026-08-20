using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Repositories;

public class FlowRepository(WcsDbContext context) : IFlowRepository
{
    private readonly WcsDbContext _context = context;

    public async Task<IEnumerable<Flow>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<FlowDbModel> query = _context.Flows.Include(f => f.Steps);
        return await query
            .Where(s => s.IsActive == true)
            .OrderBy(s => s.Area)
            .ThenBy(s => s.Name)
            .Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Flow>> GetListAsync(StageArea area, string stage, CancellationToken cancellationToken = default)
    {
        IQueryable<FlowDbModel> query = _context.Flows.Include(f => f.Steps);
        return await query
            .Where(s => s.Area == area && s.IsActive == true)
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.Name)
            .ThenBy(s => s.CreatedAt)
            .ThenBy(s => s.UpdatedAt)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Flows.Where(s => s.IsActive == true).CountAsync(cancellationToken);
    }

    public async Task<Flow?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Flows
            .Include(f => f.Steps)
            .Where(s => s.Name == name && s.IsActive == true)
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Flow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Flows
            .Include(f => f.Steps)
            .Where(s => s.Id == id && s.IsActive == true)
            .FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Flow> CreateAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        var dbModel = flow.ToDbModel();
        _context.Flows.Add(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
        return dbModel.ToDomain();
    }

    public async Task UpdateAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        // Step 1: Delete old FlowSteps physically using raw SQL
        await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM FlowSteps WHERE FlowId = {0}", 
            flow.Id);

        // Step 2: Update Flow properties using raw SQL to avoid tracking conflicts
        await _context.Database.ExecuteSqlRawAsync(
            @"UPDATE Flows 
              SET Name = {0}, 
                  Area = {1}, 
                  Priority = {2}, 
                  ReturnEmpty = {3}, 
                  UpdatedAt = {4} 
              WHERE Id = {5}",
            flow.Name,
            flow.Area.Value,
            (int)flow.Priority,
            flow.ReturnEmpty ? 1 : 0,
            DateTime.UtcNow,
            flow.Id);

        // Step 3: Insert new FlowSteps
        foreach (var step in flow.Steps)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"INSERT INTO FlowSteps (Id, FlowId, StepNo, Stage, Size, CreatedAt, UpdatedAt)
                  VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                Guid.NewGuid(),
                flow.Id,
                step.StepNo,
                step.Stage,
                step.Size.HasValue ? (int?)step.Size.Value : null,
                DateTime.UtcNow,
                DateTime.UtcNow);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingFlow = await _context.Flows.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (existingFlow is null)
        {
            throw new InvalidOperationException($"Flow với id {id} không tồn tại");
        }

        // Soft delete - set IsActive = false
        existingFlow.IsActive = false;
        existingFlow.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
