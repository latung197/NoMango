using Wcs.Common.Entities;

namespace Wcs.Common.Abstractions.Repositories;

public interface IMaterialRepository
{
    /// <summary>
    /// Lấy tất cả materials (bao gồm cả inactive)
    /// </summary>
    Task<IEnumerable<Material>> GetAllAsync(string? name, string? code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy tất cả materials có IsActive = true
    /// </summary>
    Task<IEnumerable<Material>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách materials có phân trang (chỉ lấy active)
    /// </summary>
    Task<IEnumerable<Material>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Đếm số materials active
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy material theo code (bao gồm cả inactive)
    /// </summary>
    Task<Material?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy material active theo code
    /// </summary>
    Task<Material?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy material theo id (bao gồm cả inactive)
    /// </summary>
    Task<Material?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy material active theo id
    /// </summary>
    Task<Material?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo mới material
    /// </summary>
    Task<Material> CreateAsync(Material material, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cập nhật material
    /// </summary>
    Task UpdateAsync(Material material, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Xóa vĩnh viễn material
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Soft delete material (chuyển IsActive = false)
    /// </summary>
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}




