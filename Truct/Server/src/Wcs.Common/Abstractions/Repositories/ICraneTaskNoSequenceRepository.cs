namespace Wcs.Common.Abstractions.Repositories;

public interface ICraneTaskNoSequenceRepository
{
    /// <summary>
    /// Lấy TaskNo tiếp theo trong khoảng min..max (atomic, lưu trong DB).
    /// Lần đầu khởi tạo từ MAX(TaskNo) trong CraneTaskDispatches nếu có.
    /// </summary>
    Task<int> GetNextAsync(int minValue, int maxValue, CancellationToken cancellationToken = default);
}
