using Wcs.Common.Entities;

namespace Wcs.Api.Services;

public interface IAmrLoadStateService
{
    /// <summary>
    /// AMR đang mang hàng (đã lift up). Hiện suy luận từ FlowStep; sau này có thể đọc cảm biến.
    /// </summary>
    bool IsCarryingLoad(FlowTask task);

    /// <summary>
    /// Có thể chuyển hướng sang trạm hạ hàng khác (đang mang hàng và còn bước continue trên task HIK).
    /// </summary>
    bool CanRedirectDrop(FlowTask task);
}
