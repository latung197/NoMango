using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Exceptions;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Services;

public class ErrorService(IErrorRepository errorRepository)
{
    private readonly IErrorRepository _errorRepository = errorRepository;

    public async Task<IEnumerable<Error>> GetListAsync(ErrorListRequest request, CancellationToken cancellationToken = default)
    {
        //var errors = await _errorRepository.GetListAsync(request.PageNumber, request.PageSize, cancellationToken);
        var errors = await _errorRepository.GetListAsync(request.Type != null ? (ErrorType)request.Type : null, request.Area, cancellationToken);
        return errors;
    }

    /*public async Task<IEnumerable<Error>> GetListByAreaAsync(ErrorByAreaListRequest request, CancellationToken cancellationToken = default)
    {
        var errors = await _errorRepository.ErrorByAreaListRequest(request.Area, cancellationToken);
        return errors;
    }*/

    public async Task<Error> ShowErrorAsync(string id, CancellationToken cancellationToken = default)
    {
        var error = await _errorRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Error với id {id} không tồn tại");
        return error;
    }

    public async Task<Error> CreateErrorAsync(ErrorCreateRequest request, CancellationToken cancellationToken = default)
    {
        var error = new Error(Guid.NewGuid(), request.Code, (ErrorType)request.ErrorType, request.Area, request.Stage, request.Station);
        error = await _errorRepository.CreateAsync(error, cancellationToken);
        return error;
    }

    public async Task<Error> UpdateErrorAsync(string id, ErrorUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var error = await _errorRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) ?? throw new NotFoundException($"Error với id {id} không tồn tại");
        //error.Status = request.Status;
        //error.Area = request.Area;
        //error.Area = StageArea.FromString(request.Area);
        error.UpdatedAt = DateTime.UtcNow;
        await _errorRepository.UpdateAsync(error, cancellationToken);
        return error;
    }

    public async Task<List<Error>> UpdatesErrorAsync(List<ErrorUpdateRequest> requests, CancellationToken cancellationToken = default)
    {
        if (requests == null || requests.Count == 0)
        {
            throw new Wcs.Common.Exceptions.InvalidDataException($"Không có dữ liệu gửi lên.");
        }

        var ids = requests.Select(d => d.Id).ToList();
        var errors = await _errorRepository.GetByIdsAsync(ids);

        try
        {
            foreach (var data in requests)
            {
                var error = errors.FirstOrDefault(r => r.Id == data.Id);
                if (error != null)
                {
                    // Cập nhật các trường cần thiết
                    error.Status = (ErrorStatus)data.Status;
                    error.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _errorRepository.UpdateBulkAsync(errors, cancellationToken);
        }
        catch
        {
            throw;
        }

        return errors.ToList();
    }

    public async Task DeleteErrorAsync(string id, CancellationToken cancellationToken = default)
    {
        var error = await _errorRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Error với id {id} không tồn tại");
        
        // Soft delete - set Status = 0
        error.Deactivate();
        error.UpdatedAt = DateTime.UtcNow;
        await _errorRepository.UpdateAsync(error, cancellationToken);
    }
}