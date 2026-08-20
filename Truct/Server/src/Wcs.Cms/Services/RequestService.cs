using Microsoft.EntityFrameworkCore;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.Exceptions;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Services;

public class RequestService(IRequestRepository requestRepository, IMaterialRepository materialRepository)
{
    private readonly IRequestRepository _requestRepository = requestRepository;
    private readonly IMaterialRepository _materialRepository = materialRepository;

    private static bool CanEditMaterials(RequestStep currentStep) =>
        currentStep is RequestStep.Initial or RequestStep.Requested;

    private async Task<Request> GetEditableRequestAsync(Guid requestId, CancellationToken cancellationToken)
    {
        var request = await _requestRepository.GetActiveByIdAsync(requestId, cancellationToken)
            ?? throw new NotFoundException($"Request với id {requestId} không tồn tại");

        if (!CanEditMaterials(request.CurrentStep))
        {
            throw new Wcs.Common.Exceptions.InvalidDataException($"Request '{request.Code}' không thể thay đổi nguyên vật liệu ở trạng thái hiện tại");
        }

        return request;
    }

    /// <summary>
    /// Lấy tất cả requests (chỉ lấy những request có IsActive = true)
    /// </summary>
    public async Task<IEnumerable<Request>> GetAllAsync(RequestListRequest request, CancellationToken cancellationToken = default)
    {
        return await _requestRepository.GetAllAsync(request.Stage, cancellationToken);
    }

    /// <summary>
    /// Lấy chi tiết request theo id (chỉ lấy request có IsActive = true)
    /// </summary>
    public async Task<Request> ShowRequestAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Request với id {id} không tồn tại");
        return request;
    }

    /// <summary>
    /// Lấy chi tiết request theo code
    /// </summary>
    public async Task<Request?> GetRequestByCode(string code, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByCodeAsync(code, cancellationToken)
            ?? throw new NotFoundException($"Request với mã {code} không tồn tại");
        return request;
    }

    /// <summary>
    /// Tạo request mới (IsActive mặc định là true)
    /// </summary>
    public async Task<Request> CreateRequestAsync(RequestCreateRequest data, CancellationToken cancellationToken = default)
    {
        if (data == null || data.Materials == null || data.Materials.Count == 0)
        {
            throw new Wcs.Common.Exceptions.InvalidDataException($"Không có dữ liệu gửi lên.");
        }

        // Tạo request mới
        var request = new Request(
            code: data.Code,
            stage: data.StageCode,
            deliveryCode: null,
            deliveryOrder: null,
            //deliveryCode: data.DeliveryCode,
            //deliveryOrder: data.DeliveryOrder,
            currentStep: RequestStep.Requested,
            note: "",
            isActive: true  // Mặc định là true khi tạo mới
        );

        try
        {
            request = await _requestRepository.CreateAsync(request, cancellationToken);

            // TODO: Validate materials exist

            var materialRequests = data.Materials.Select(mr => new MaterialRequest
            {
                RequestId = request.Id,
                MaterialId = mr.MaterialId,
                Quantity = mr.Quantity,
                Note = mr.Note
            }).ToList();

            request.Materials = (await _requestRepository.CreateBulkAsync(materialRequests, cancellationToken)).ToList();

        }
        catch (DbUpdateException ex)
        {
            // Handle database constraint violations (e.g., unique index)
            if (ex.InnerException?.Message.Contains("IX_Requests_Code_IsActive") == true || 
                ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Request với mã {request.Code} đã tồn tại");
            }
            throw;
        }
        
        return request;
    }

    /// <summary>
    /// Cập nhật request
    /// </summary>
    public async Task<Request> UpdateRequestAsync(string id, RequestUpdateRequest data, CancellationToken cancellationToken = default)
    {
        // Cho phép update cả request có IsActive = 0 (để có thể khôi phục)
        var request = await _requestRepository.GetByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Request với id {id} không tồn tại");
        
        // Kiểm tra nếu đổi code hoặc đang khôi phục (IsActive từ 0 -> 1)
        // thì code mới có trùng với request active khác không (trừ chính nó)
        if (request.Code != data.Code || (request.IsActive == false && data.IsActive == true))
        {
            var existing = await _requestRepository.GetActiveByCodeAsync(request.Code, cancellationToken);
            // Nếu tìm thấy một request active khác với code này (không phải chính nó)
            if (existing != null && existing.Id != request.Id)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Request với mã {request.Code} đã tồn tại");
            }
        }

        if (data.FlowTaskId != null)
        {
            request.FlowTaskId = data.FlowTaskId;
        }
        request.Code = data.Code;
        //request.Stage = data.Stage;
        request.StageCode = data.StageCode;
        request.DeliveryCode = data.DeliveryCode;
        request.DeliveryOrder = data.DeliveryOrder;
        request.CurrentStep = data.CurrentStep;
        request.Note = data.Note;
        request.IsActive = request.IsActive;
        request.UpdatedAt = DateTime.UtcNow;
        
        try
        {
            await _requestRepository.UpdateAsync(request, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Handle database constraint violations (e.g., unique index)
            // Chỉ báo lỗi nếu có request active khác với code này
            if (ex.InnerException?.Message.Contains("IX_Requests_Code_IsActive") == true || 
                ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                throw new Wcs.Common.Exceptions.InvalidDataException($"Request với mã {request.Code} đã tồn tại");
            }
            throw;
        }

        if (data.CurrentStep == RequestStep.Delivered && !string.IsNullOrWhiteSpace(request.DeliveryCode) && request.DeliveryOrder != null && request.DeliveryOrder > 0)
        {
            var requestNext = await _requestRepository.GetByDeliveryAsync(request.DeliveryCode, (int)request.DeliveryOrder, cancellationToken);
            if (requestNext != null)
            {
                //Request đã được giao thành công, Robot sẽ tiếp tục giao đến điểm tiếp theo. DeliveryCode: { request.DeliveryCode}, DeliveryOrder: { request.DeliveryOrder}
                requestNext.Note = "DeliverNextPoint";
                return requestNext;
            }
        }
        
        return request;
    }

    /// <summary>
    /// Cập nhật requests
    /// </summary>
    public async Task<List<Request>> UpdatesRequestAsync(List<RequestUpdateRequest> datas, CancellationToken cancellationToken = default)
    {
        if (datas == null || datas.Count == 0)
        {
            throw new Wcs.Common.Exceptions.InvalidDataException($"Không có dữ liệu gửi lên.");
        }

        var ids = datas.Select(d => d.Id).ToList();
        var requests = await _requestRepository.GetRequestsByIdsAsync(ids);

        try
        {
            foreach (var data in datas)
            {
                var request = requests.FirstOrDefault(r => r.Id == data.Id);
                if (request != null)
                {
                    // Cập nhật các trường cần thiết
                    if (data.FlowTaskId != null)
                    {
                        request.FlowTaskId = data.FlowTaskId;
                    }
                    request.DeliveryCode = data.DeliveryCode;
                    request.DeliveryOrder = data.DeliveryOrder;
                    request.CurrentStep = data.CurrentStep;
                    //request.Note = data.Note;
                    //request.IsActive = request.IsActive;
                    request.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _requestRepository.UpdateBulkAsync(requests, cancellationToken);
        }
        catch
        {
            throw;
        }

        return requests.ToList();
    }

    /// <summary>
    /// Thêm nguyên vật liệu vào request (chỉ khi request ở trạng thái Chưa chuẩn bị)
    /// </summary>
    public async Task<MaterialRequest> AddMaterialRequestAsync(string requestId, MaterialRequestDto data, CancellationToken cancellationToken = default)
    {
        var request = await GetEditableRequestAsync(Guid.Parse(requestId), cancellationToken);

        var material = await _materialRepository.GetActiveByIdAsync(data.MaterialId, cancellationToken)
            ?? throw new NotFoundException($"Vật liệu với id {data.MaterialId} không tồn tại");

        if (await _requestRepository.HasMaterialInRequestAsync(request.Id, material.Id, cancellationToken: cancellationToken))
        {
            throw new Wcs.Common.Exceptions.InvalidDataException($"Vật liệu '{material.Code}' đã tồn tại trong yêu cầu");
        }

        var materialRequest = new MaterialRequest(
            request.Id,
            material.Id,
            data.Quantity,
            data.Note);

        return await _requestRepository.CreateMaterialRequestAsync(materialRequest, cancellationToken);
    }

    /// <summary>
    /// Cập nhật nguyên vật liệu trong request (chỉ khi request ở trạng thái Chưa chuẩn bị)
    /// </summary>
    public async Task<MaterialRequest> UpdateMaterialRequestAsync(string requestId, string materialRequestId, MaterialRequestDto data, CancellationToken cancellationToken = default)
    {
        await GetEditableRequestAsync(Guid.Parse(requestId), cancellationToken);

        var materialRequest = await _requestRepository.GetMaterialRequestByIdAsync(Guid.Parse(materialRequestId), cancellationToken)
            ?? throw new NotFoundException($"Nguyên vật liệu yêu cầu với id {materialRequestId} không tồn tại");

        if (materialRequest.RequestId != Guid.Parse(requestId))
        {
            throw new Wcs.Common.Exceptions.InvalidDataException("Nguyên vật liệu không thuộc yêu cầu này");
        }

        materialRequest.Quantity = data.Quantity;
        materialRequest.Note = data.Note;
        materialRequest.UpdatedAt = DateTime.UtcNow;

        await _requestRepository.UpdateMaterialRequestAsync(materialRequest, cancellationToken);

        return await _requestRepository.GetMaterialRequestByIdAsync(materialRequest.Id, cancellationToken)
            ?? materialRequest;
    }

    /// <summary>
    /// Xóa nguyên vật liệu khỏi request (chỉ khi request ở trạng thái Chưa chuẩn bị)
    /// </summary>
    public async Task DeleteMaterialRequestAsync(string requestId, string materialRequestId, CancellationToken cancellationToken = default)
    {
        await GetEditableRequestAsync(Guid.Parse(requestId), cancellationToken);

        var materialRequest = await _requestRepository.GetMaterialRequestByIdAsync(Guid.Parse(materialRequestId), cancellationToken)
            ?? throw new NotFoundException($"Nguyên vật liệu yêu cầu với id {materialRequestId} không tồn tại");

        if (materialRequest.RequestId != Guid.Parse(requestId))
        {
            throw new Wcs.Common.Exceptions.InvalidDataException("Nguyên vật liệu không thuộc yêu cầu này");
        }

        await _requestRepository.DeleteMaterialRequestAsync(materialRequest.Id, cancellationToken);
    }

    /// <summary>
    /// Xóa request update sản phẩn = 0
    /// </summary>
    public async Task DeleteRequestAsync(string id, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetActiveByIdAsync(Guid.Parse(id), cancellationToken) 
            ?? throw new NotFoundException($"Request với id {id} không tồn tại");

        if (request.CurrentStep != RequestStep.Initial && request.CurrentStep != RequestStep.Requested)
        {
            throw new NotFoundException($"Request '{request.Code}' không thể xóa do đang được chuẩn bị");
        }
        
        await _requestRepository.SoftDeleteAsync(request.Id, cancellationToken);
    }
}
