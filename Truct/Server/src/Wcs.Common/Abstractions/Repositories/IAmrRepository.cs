using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions.Repositories;

public interface IAmrRepository
{
    /// <summary>
    /// Lấy tất cả AMR (bao gồm cả inactive)
    /// </summary>
    Task<IEnumerable<Amr>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách AMR không phân trang (chỉ lấy active)
    /// </summary>
    Task<IEnumerable<Amr>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy AMR active theo area (chỉ lấy active)
    /// </summary>
    Task<IEnumerable<Amr>> AmrByAreaListRequest(string? area, CancellationToken cancellationToken = default);

    /// <summary>
    /// Đếm số AMR active
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy AMR theo code (bao gồm cả inactive)
    /// </summary>
    Task<Amr?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy AMR theo code và area (bao gồm cả inactive)
    /// </summary>
    /// <returns></returns>
    Task<Amr?> GetByCodeAndAreaAsync(string code, StageArea area, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy AMR theo Id (bao gồm cả inactive)
    /// </summary>
    Task<Amr?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo mới AMR
    /// </summary>
    Task<Amr> CreateAsync(Amr amr, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tạo mới AMRs
    /// </summary>
    Task<IEnumerable<Amr>> CreateBulkAsync(IEnumerable<Amr> amrs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cập nhật AMR
    /// </summary>
    Task UpdateAsync(Amr amr, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cập nhật AMRs
    /// </summary>
    Task UpdateBulkAsync(IEnumerable<Amr> amrs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activate AMR
    /// </summary>
    //Task<IEnumerable<Amr>> Activate(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivate AMR
    /// </summary>
    Task<IEnumerable<Amr>> DeactivateAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Amr>> DeactivateAsync(IEnumerable<Amr> amrs, CancellationToken cancellationToken = default);
}