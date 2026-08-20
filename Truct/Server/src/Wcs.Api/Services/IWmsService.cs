using Wcs.Common.ValueObjects;

namespace Wcs.Api.Services;

public interface IWmsService
{
    Task ConfirmPositionInboundAsync(
        string position,
        string stageCode,
        string cassetteId,
        string size,
        int quantity,
        string product,
        CancellationToken ct = default);

    Task CompleteTransferInboundAsync(string position, CancellationToken ct = default);

    Task<IReadOnlyList<CranePosition>> FindEmptyInboundPositionsAsync(CancellationToken ct = default);

    /// <summary>
    /// Kiểm tra WMS còn ô nhập kho trống. Trả về null nếu kho full hoặc WMS lỗi (không throw).
    /// </summary>
    Task<IReadOnlyList<CranePosition>?> TryFindEmptyInboundPositionsAsync(CancellationToken ct = default);

    Task<string> FindReplacementInboundPositionAsync(
        string failedPosition,
        CancellationToken ct = default);

    Task HandleErrorInboundAsync(string failedPosition, string replacementPosition, CancellationToken ct = default);

    Task UpdatePositionStatusAsync(string position, int positionStatus, CancellationToken ct = default);

    Task ConfirmPositionOutboundAsync(string position, CancellationToken ct = default);

    Task CompleteTransferOutboundAsync(string position, CancellationToken ct = default);

    Task<WarehouseOutboundPosition?> FindFifoOutboundPositionAsync(
        string stageCode,
        CancellationToken ct = default);

    Task<WarehouseOutboundPosition?> FindEmptyTrayOutboundPositionAsync(
        CancellationToken ct = default);
}
