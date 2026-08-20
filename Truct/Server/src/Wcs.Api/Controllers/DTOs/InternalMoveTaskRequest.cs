using System.ComponentModel.DataAnnotations;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Request DTO cho task di chuyển nội bộ trong kho (Internal Move)
/// </summary>
public class InternalMoveTaskRequest
{
    /// <summary>Vị trí lấy hàng (Pick Position)</summary>
    [Required(ErrorMessage = "PickPosition là bắt buộc")]
    public CranePositionRequest PickPosition { get; set; } = null!;

    /// <summary>Vị trí đặt hàng (Put Position)</summary>
    [Required(ErrorMessage = "PutPosition là bắt buộc")]
    public CranePositionRequest PutPosition { get; set; } = null!;
}
