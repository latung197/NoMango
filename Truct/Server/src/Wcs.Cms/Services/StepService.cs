using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Exceptions;

namespace Wcs.Cms.Services;

public class StepService(IStepRepository stepRepository)
{
    private readonly IStepRepository _stepRepository = stepRepository;

    public async Task<IEnumerable<Step>> GetListAsync(StepListRequest request, CancellationToken cancellationToken = default)
    {
        var steps = await _stepRepository.GetListAsync(request.PageNumber, request.PageSize, cancellationToken);
        return steps;
    }

    public async Task<Step> ShowStepAsync(string id, CancellationToken cancellationToken = default)
    {
        var step = await _stepRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Step với id {id} không tồn tại");
        return step;
    }

    public async Task<Step> CreateStepAsync(StepCreateRequest request, CancellationToken cancellationToken = default)
    {
        var step = new Step(Guid.NewGuid(), Guid.Parse(request.FlowId), request.StepNo, request.Stage);
        step = await _stepRepository.CreateAsync(step, cancellationToken);
        return step;
    }

    public async Task<Step> UpdateStepAsync(string id, StepUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var step = await _stepRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Step với id {id} không tồn tại");
        step.StepNo = request.StepNo;
        step.Stage = request.Stage;
        await _stepRepository.UpdateAsync(step, cancellationToken);
        return step;
    }
}