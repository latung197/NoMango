using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Repositories;

public class StepRepository(WcsDbContext context) : IStepRepository
{
    private readonly WcsDbContext _context = context;

    public async Task<IEnumerable<Step>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<StepDbModel> query = _context.Steps;
        return await query
            .OrderBy(s => s.StepNo)
            .ThenBy(s => s.Stage)
            .Paginate(page, pageSize)
            .Select(dbModel => dbModel.ToDomain())
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Steps.CountAsync(cancellationToken);
    }

    public async Task<Step?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Steps.Where(s => s.Stage == name).FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Step?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.Steps.Where(s => s.Id == id).FirstOrDefaultAsync(cancellationToken);
        return dbModel?.ToDomain() ?? null;
    }

    public async Task<Step> CreateAsync(Step step, CancellationToken cancellationToken = default)
    {
        _context.Steps.Add(step.ToDbModel());
        await _context.SaveChangesAsync(cancellationToken);
        return step;
    }

    public async Task UpdateAsync(Step step, CancellationToken cancellationToken = default)
    {
        var existingStep = await _context.Steps.FirstOrDefaultAsync(s => s.Id == step.Id, cancellationToken);
        if (existingStep is null)
        {
            throw new InvalidOperationException($"Step với id {step.Id} không tồn tại");
        }

        existingStep.FlowId = step.FlowId;
        existingStep.StepNo = step.StepNo;
        existingStep.Stage = step.Stage;
        existingStep.Size = step.Size;
        await _context.SaveChangesAsync(cancellationToken);
    }
}