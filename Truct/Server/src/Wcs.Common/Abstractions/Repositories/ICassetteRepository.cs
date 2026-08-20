using Wcs.Common.Entities;

namespace Wcs.Common.Abstractions.Repositories;

public interface ICassetteRepository
{
    /// <summary>
    /// Lấy tất cả cassettes (bao gồm cả inactive)
    /// </summary>
    Task<IEnumerable<Cassette>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy tất cả cassettes có IsActive = true
    /// </summary>
    Task<IEnumerable<Cassette>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy danh sách cassettes có phân trang (chỉ lấy active)
    /// </summary>
    Task<IEnumerable<Cassette>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Đếm số cassettes active
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy cassette theo code (bao gồm cả inactive)
    /// </summary>
    Task<Cassette?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy cassette active theo code
    /// </summary>
    Task<Cassette?> GetActiveByCodeAsync(string code, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy cassette theo id (bao gồm cả inactive)
    /// </summary>
    Task<Cassette?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy cassette active theo id
    /// </summary>
    Task<Cassette?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tạo mới cassette
    /// </summary>
    Task<Cassette> CreateAsync(Cassette cassette, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cập nhật cassette
    /// </summary>
    Task UpdateAsync(Cassette cassette, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Xóa vĩnh viễn cassette
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Soft delete cassette (chuyển IsActive = false)
    /// </summary>
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}




