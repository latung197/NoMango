using Wcs.Common.Entities;

namespace Wcs.Common.Abstractions.Repositories;

public interface IRequestRepository
{
    /// <summary>
    /// Lấy tất cả requests (bao gồm cả inactive)
    /// </summary>
    Task<IEnumerable<Request>> GetAllAsync(string? stage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy tất cả requests có IsActive = true
    /// </summary>
    //Task<IEnumerable<Request>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách requests có phân trang (chỉ lấy active)
    /// </summary>
    //Task<IEnumerable<Request>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Đếm số requests active
    /// </summary>
    //Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy request theo delivery (bao gồm cả inactive)
    /// </summary>
    Task<Request?> GetByDeliveryAsync(string deliveryCode, int deliveryOrder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy request theo code (bao gồm cả inactive)
    /// </summary>
    Task<Request?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy request active theo code
    /// </summary>
    Task<Request?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy request theo id (bao gồm cả inactive)
    /// </summary>
    Task<Request?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy request active theo id
    /// </summary>
    Task<Request?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy requests theo ids
    /// </summary>
    Task<IEnumerable<Request>> GetRequestsByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo mới request
    /// </summary>
    Task<Request> CreateAsync(Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo mới request
    /// </summary>
    /// <param name="requests"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<MaterialRequest>> CreateBulkAsync(IEnumerable<MaterialRequest> requests, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy material request theo id
    /// </summary>
    Task<MaterialRequest?> GetMaterialRequestByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra vật liệu đã tồn tại trong request chưa
    /// </summary>
    Task<bool> HasMaterialInRequestAsync(Guid requestId, Guid materialId, Guid? excludeMaterialRequestId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo material request
    /// </summary>
    Task<MaterialRequest> CreateMaterialRequestAsync(MaterialRequest materialRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cập nhật material request
    /// </summary>
    Task UpdateMaterialRequestAsync(MaterialRequest materialRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa material request
    /// </summary>
    Task DeleteMaterialRequestAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cập nhật request
    /// </summary>
    Task UpdateAsync(Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cập nhật requests
    /// </summary>
    /// <returns></returns>
    Task UpdateBulkAsync(IEnumerable<Request> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa vĩnh viễn request
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Soft delete request (chuyển IsActive = false)
    /// </summary>
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}




