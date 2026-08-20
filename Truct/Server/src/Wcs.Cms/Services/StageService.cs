using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.ValueObjects;
using Wcs.Common.Exceptions;

namespace Wcs.Cms.Services;

public class StageService(IStageRepository stageRepository)
{
    private readonly IStageRepository _stageRepository = stageRepository;

    public async Task<IEnumerable<Stage>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var stages = await _stageRepository.GetListAsync(cancellationToken);
        return stages;
    }

    public async Task<IEnumerable<Stage>> GetListByAreaAsync(StageByAreaListRequest request, CancellationToken cancellationToken = default)
    {
        var stages = await _stageRepository.StageByAreaListRequest(request.Area, cancellationToken);
        return stages;
    }

    public async Task<Stage> ShowStageAsync(string id, CancellationToken cancellationToken = default)
    {
        var stage = await _stageRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Stage với id {id} không tồn tại");
        return stage;
    }

    public async Task<Stage> CreateStageAsync(StageCreateRequest request, CancellationToken cancellationToken = default)
    {
        // Kiểm tra mã công đoạn đã tồn tại trong khu vực chưa (với IsActive = 1)
        var area = StageArea.FromString(request.Area);
        var existingStage = await _stageRepository.GetByCodeAndAreaAsync(request.Code, area, cancellationToken);
        if (existingStage != null)
        {
            throw new ValidationException($"Mã công đoạn '{request.Code}' đã tồn tại trong khu vực '{request.Area}'");
        }

        float distance = 1;
        if (request.Distance != null)
        {
            distance = (float)request.Distance;
        }
        var stage = new Stage(Guid.NewGuid(), request.Name, request.Code, area, distance);
        stage = await _stageRepository.CreateAsync(stage, cancellationToken);
        return stage;
    }

    public async Task<Stage> UpdateStageAsync(string id, StageUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var stage = await _stageRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Stage với id {id} không tồn tại");
        
        var newArea = StageArea.FromString(request.Area);
        
        // Kiểm tra mã công đoạn đã tồn tại trong khu vực chưa (nếu đổi code hoặc đổi area)
        if (stage.Code != request.Code || stage.Area.Value != newArea.Value)
        {
            var existingStage = await _stageRepository.GetByCodeAndAreaAsync(request.Code, newArea, cancellationToken);
            if (existingStage != null && existingStage.Id != stage.Id)
            {
                throw new ValidationException($"Mã công đoạn '{request.Code}' đã tồn tại trong khu vực '{request.Area}'");
            }
        }

        stage.Name = request.Name;
        stage.Code = request.Code;
        stage.Area = newArea;
        if (request.Distance != null)
        {
            stage.Distance = (float)request.Distance;
        }
        stage.UpdatedAt = DateTime.UtcNow;
        await _stageRepository.UpdateAsync(stage, cancellationToken);
        return stage;
    }

    public async Task DeleteStageAsync(string id, CancellationToken cancellationToken = default)
    {
        var stage = await _stageRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Stage với id {id} không tồn tại");
        
        // Soft delete - set Status = 0
        stage.Deactivate();
        stage.UpdatedAt = DateTime.UtcNow;
        await _stageRepository.UpdateAsync(stage, cancellationToken);
    }
}