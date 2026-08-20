using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Abstractions.Repositories;

public interface ITransferRequestRepository
{
    Task<TransferRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TransferRequest> CreateAsync(TransferRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(TransferRequest request, CancellationToken cancellationToken = default);
    Task<TransferRequest?> ClaimWaitingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveAsync(
        TransferRequestType type,
        string fromStageCode,
        string toStageCode,
        Size size,
        bool isEmptyTray,
        string stationCode,
        string? cassetteCode = null,
        CancellationToken cancellationToken = default);
    Task<TransferRequest?> FindWaitingCounterpartAsync(
        TransferRequestType counterpartType,
        string fromStageCode,
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsWaitingEmptyTrayReceiveAsync(
        string toStationCode,
        Size size,
        CancellationToken cancellationToken = default);
    Task<TransferRequest?> FindWaitingEmptyTraySendToStageAsync(
        string toStageCode,
        Size size,
        CancellationToken cancellationToken = default);
    Task<TransferRequest?> FindWaitingSendToStageAsync(
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default);
    Task<TransferRequest?> FindWaitingAutoOpcReceiveAtStageAsync(
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> FindDispatchingSendsToStageAsync(
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default);
    Task<TransferRequest?> FindWaitingEmptyTrayReceiveAtStageAsync(
        string toStageCode,
        Size size,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> GetWaitingByStageAsync(string stageCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> GetAllWaitingAsync(CancellationToken cancellationToken = default);
    Task<TransferRequest?> CancelIfPendingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> GetExpiredSendRequestsAsync(DateTime utcNow, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveReceiveOnStationAsync(string toStationCode, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveSendOnStationAsync(string fromStationCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> GetPendingWarehouseRequestsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> GetWaitingReceiveForWarehouseRetryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> GetWaitingEmptyTrayReceivesForWarehouseRetryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRequest>> GetDispatchingOlderThanAsync(
        DateTime updatedBeforeUtc,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteIfDispatchingAsync(Guid id, CancellationToken cancellationToken = default);
}
