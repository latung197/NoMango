using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Exceptions;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Services;

public class FlowService(IFlowRepository flowRepository)
{
    private readonly IFlowRepository _flowRepository = flowRepository;

    private static Step CreateStepFromRequest(Guid flowId, StepRequest stepReq)
    {
        var step = new Step(Guid.NewGuid(), flowId, stepReq.Step, stepReq.Stage);
        if (stepReq.Size.HasValue && Enum.IsDefined(typeof(Size), stepReq.Size.Value))
        {
            step.Size = (Size)stepReq.Size.Value;
        }
        return step;
    }

    public async Task<IEnumerable<Flow>> GetListAsync(FlowListRequest request, CancellationToken cancellationToken = default)
    {
        var flows = await _flowRepository.GetListAsync(request.PageNumber, request.PageSize, cancellationToken);
        return flows;
    }

    public async Task<IEnumerable<Flow>> GetListAsync(FlowSearchRequest request, CancellationToken cancellationToken = default)
    {
        var flows = await _flowRepository.GetListAsync(StageArea.FromString(request.Area), request.Stage, cancellationToken);
        return flows;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _flowRepository.CountAsync(cancellationToken);
    }

    public async Task<Flow> ShowFlowAsync(string id, CancellationToken cancellationToken = default)
    {
        var flow = await _flowRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Flow với id {id} không tồn tại");
        return flow;
    }

    public async Task<Flow> CreateFlowAsync(FlowCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Map priority string to enum
        var priority = request.Priority switch
        {
            "Cao" => FlowPriority.High,
            "Trung" => FlowPriority.Medium,
            "Thấp" => FlowPriority.Low,
            _ => FlowPriority.Medium
        };
        
        var flow = new Flow(Guid.NewGuid(), request.Name, StageArea.FromString(request.Area), priority, request.ReturnEmpty, FlowStatus.Active);
        
        // Add steps if provided
        if (request.Steps != null && request.Steps.Count > 0)
        {
            foreach (var stepReq in request.Steps)
            {
                flow.Steps.Add(CreateStepFromRequest(flow.Id, stepReq));
            }
        }

        flow = await _flowRepository.CreateAsync(flow, cancellationToken);
        return flow;
    }

    public async Task<Flow> UpdateFlowAsync(string id, FlowUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var flow = await _flowRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Flow với id {id} không tồn tại");
        
        // Map priority string to enum
        var priority = request.Priority switch
        {
            "Cao" => FlowPriority.High,
            "Trung" => FlowPriority.Medium,
            "Thấp" => FlowPriority.Low,
            _ => FlowPriority.Medium
        };
        
        flow.Name = request.Name;
        flow.Area = StageArea.FromString(request.Area);
        flow.Priority = priority;
        flow.ReturnEmpty = request.ReturnEmpty;

        // Update steps - clear and add new
        flow.Steps.Clear();
        if (request.Steps != null && request.Steps.Count > 0)
        {
            foreach (var stepReq in request.Steps)
            {
                flow.Steps.Add(CreateStepFromRequest(flow.Id, stepReq));
            }
        }

        await _flowRepository.UpdateAsync(flow, cancellationToken);
        return flow;
    }

    public async Task DeleteFlowAsync(string id, CancellationToken cancellationToken = default)
    {
        var flow = await _flowRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Flow với id {id} không tồn tại");
        await _flowRepository.DeleteAsync(flow.Id, cancellationToken);
    }
}
