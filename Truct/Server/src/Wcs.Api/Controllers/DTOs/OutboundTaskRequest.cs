using System.ComponentModel.DataAnnotations;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Request DTO cho task xuất kho thủ công (Outbound)
/// </summary>
public class OutboundTaskRequest
{
    /// <summary>Vị trí lấy hàng trong kho (Pick Position)</summary>
    [Required(ErrorMessage = "PickPosition là bắt buộc")]
    public CranePositionRequest PickPosition { get; set; } = null!;

    // ------------------------------------------------------------------
    // WMS – ConfirmPositionOutbound (gọi trước khi crane lấy hàng)
    // ------------------------------------------------------------------

    /// <summary>Gọi WMS ConfirmPositionOutbound trước khi gửi lệnh crane. Mặc định: true</summary>
    public bool CallWmsConfirm { get; set; } = true;

    // ------------------------------------------------------------------
    // WMS – CompleteTransferOutbound (gọi sau khi crane hoàn thành)
    // ------------------------------------------------------------------

    /// <summary>Gọi WMS CompleteTransferOutbound khi crane báo thành công. Mặc định: true</summary>
    public bool CallWmsComplete { get; set; } = true;
}

