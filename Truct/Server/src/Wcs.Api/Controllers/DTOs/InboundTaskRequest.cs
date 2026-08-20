using System.ComponentModel.DataAnnotations;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Request DTO cho task nhập kho thủ công (Inbound)
/// </summary>
public class InboundTaskRequest
{
    /// <summary>Vị trí đặt hàng trong kho (Put Position)</summary>
    [Required(ErrorMessage = "PutPosition là bắt buộc")]
    public CranePositionRequest PutPosition { get; set; } = null!;

    // ------------------------------------------------------------------
    // WMS – ConfirmPositionInbound (gọi trước khi crane lấy hàng)
    // ------------------------------------------------------------------

    /// <summary>Gọi WMS ConfirmPositionInbound sau khi crane nhận lệnh. Mặc định: true</summary>
    public bool CallWmsConfirm { get; set; } = true;

    /// <summary>Mã stage (khu vực lưu hàng). Bắt buộc khi CallWmsConfirm = true</summary>
    public string? StageCode { get; set; }

    /// <summary>
    /// ID cassette/pallet.
    /// Trong luồng nhập kho thủ công, để trống — hệ thống sẽ tự đọc từ OPC QR scanner
    /// ngay sau bước Station handshake (Step 1).
    /// </summary>
    public string? CassetteId { get; set; }

    /// <summary>Kích thước (size)</summary>
    public string? Size { get; set; }

    /// <summary>Số lượng</summary>
    public int Quantity { get; set; } = 1;

    /// <summary>Mã sản phẩm</summary>
    public string? Product { get; set; }

    // ------------------------------------------------------------------
    // WMS – CompleteTransferInbound (gọi sau khi crane hoàn thành)
    // ------------------------------------------------------------------

    /// <summary>Gọi WMS CompleteTransferInbound khi crane báo thành công. Mặc định: true</summary>
    public bool CallWmsComplete { get; set; } = true;
}
