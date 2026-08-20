using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Services;

public class FlowTaskService(IFlowTasksRepository flowTaskRepository)
{
    private readonly IFlowTasksRepository _flowTaskRepository = flowTaskRepository;

    public async Task<IEnumerable<FlowTask>> GetListAsync(FlowTaskListRequest request, CancellationToken cancellationToken = default)
    {
        var flowsTask = await _flowTaskRepository.GetListAsync(request.PageNumber, request.PageSize, cancellationToken);
        return flowsTask;
    }

    public async Task<IEnumerable<FlowTask>> GetListAsync(FlowSearchRequest request, CancellationToken cancellationToken = default)
    {
        var flowsTask = await _flowTaskRepository.GetListAsync(StageArea.FromString(request.Area), request.Stage, cancellationToken);
        return flowsTask;
    }

    public Task<IEnumerable<FlowTask>> GetCancelledTasksAsync(int limit = 50, CancellationToken cancellationToken = default)
        => _flowTaskRepository.GetCancelledTasksAsync(limit, cancellationToken);

    /*public async Task<FlowTask> ShowFlowTaskAsync(string id, CancellationToken cancellationToken = default)
    {
        var flowsTask = await _flowTaskRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Flow Task với id {id} không tồn tại");
        return flowsTask;
    }*/
}