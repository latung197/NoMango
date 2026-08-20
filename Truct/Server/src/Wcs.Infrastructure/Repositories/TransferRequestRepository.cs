using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Mappers;

namespace Wcs.Infrastructure.Repositories;

public class TransferRequestRepository(WcsDbContext context) : ITransferRequestRepository
{
    private readonly WcsDbContext _context = context;

    public async Task<TransferRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.TransferRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return dbModel?.ToDomain();
    }

    public async Task<TransferRequest> CreateAsync(TransferRequest request, CancellationToken cancellationToken = default)
    {
        var dbModel = request.ToDbModel();
        _context.TransferRequests.Add(dbModel);
        await _context.SaveChangesAsync(cancellationToken);
        _context.Entry(dbModel).State = EntityState.Detached;
        return dbModel.ToDomain();
    }

    public async Task UpdateAsync(TransferRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _context.TransferRequests
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException($"TransferRequest {request.Id} không tồn tại");

        var updated = request.ToDbModel();
        updated.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existing).CurrentValues.SetValues(updated);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TransferRequest?> ClaimWaitingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var updated = await _context.TransferRequests
            .Where(r => r.Id == id && r.Status == TransferRequestStatus.Waiting)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.Status, TransferRequestStatus.Dispatching)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        if (updated == 0)
        {
            return null;
        }

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsActiveAsync(
        TransferRequestType type,
        string fromStageCode,
        string toStageCode,
        Size size,
        bool isEmptyTray,
        string stationCode,
        string? cassetteCode = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TransferRequests.Where(r =>
            (r.Status == TransferRequestStatus.Waiting || r.Status == TransferRequestStatus.Dispatching) &&
            r.Type == type &&
            r.FromStageCode == fromStageCode &&
            r.ToStageCode == toStageCode &&
            r.Size == size &&
            r.IsEmptyTray == isEmptyTray);

        query = type == TransferRequestType.Send
            ? query.Where(r => r.FromStationCode == stationCode)
            : query.Where(r => r.ToStationCode == stationCode);

        if (!string.IsNullOrWhiteSpace(cassetteCode))
        {
            query = query.Where(r => r.CassetteCode == cassetteCode);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<TransferRequest?> FindWaitingCounterpartAsync(
        TransferRequestType counterpartType,
        string fromStageCode,
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.Type == counterpartType &&
                r.FromStageCode == fromStageCode &&
                r.ToStageCode == toStageCode &&
                r.Size == size &&
                r.IsEmptyTray == isEmptyTray)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<bool> ExistsWaitingEmptyTrayReceiveAsync(
        string toStationCode,
        Size size,
        CancellationToken cancellationToken = default)
    {
        return await _context.TransferRequests.AnyAsync(r =>
            r.Status == TransferRequestStatus.Waiting &&
            r.Type == TransferRequestType.Receive &&
            r.IsEmptyTray &&
            r.ToStationCode == toStationCode &&
            r.Size == size,
            cancellationToken);
    }

    public async Task<TransferRequest?> FindWaitingEmptyTraySendToStageAsync(
        string toStageCode,
        Size size,
        CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.Type == TransferRequestType.Send &&
                r.IsEmptyTray &&
                r.ToStageCode == toStageCode &&
                r.Size == size)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<TransferRequest?> FindWaitingSendToStageAsync(
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.Type == TransferRequestType.Send &&
                r.ToStageCode == toStageCode &&
                r.Size == size &&
                r.IsEmptyTray == isEmptyTray)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<TransferRequest?> FindWaitingAutoOpcReceiveAtStageAsync(
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.Type == TransferRequestType.Receive &&
                r.Source == TransferRequestSource.AutoOpc &&
                r.ToStageCode == toStageCode &&
                r.Size == size &&
                r.IsEmptyTray == isEmptyTray &&
                !r.IsWipTray)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<IReadOnlyList<TransferRequest>> FindDispatchingSendsToStageAsync(
        string toStageCode,
        Size size,
        bool isEmptyTray,
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Dispatching &&
                r.Type == TransferRequestType.Send &&
                r.ToStageCode == toStageCode &&
                r.Size == size &&
                r.IsEmptyTray == isEmptyTray)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<TransferRequest?> FindWaitingEmptyTrayReceiveAtStageAsync(
        string toStageCode,
        Size size,
        CancellationToken cancellationToken = default)
    {
        var dbModel = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.Type == TransferRequestType.Receive &&
                r.IsEmptyTray &&
                r.ToStageCode == toStageCode &&
                r.Size == size)
            .OrderBy(r => r.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return dbModel?.ToDomain();
    }

    public async Task<IReadOnlyList<TransferRequest>> GetWaitingByStageAsync(
        string stageCode,
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .Where(r =>
                (r.Status == TransferRequestStatus.Waiting || r.Status == TransferRequestStatus.Dispatching) &&
                (r.FromStageCode == stageCode || r.ToStageCode == stageCode))
            .OrderBy(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TransferRequest>> GetAllWaitingAsync(
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .AsNoTracking()
            .Where(r => r.Status == TransferRequestStatus.Waiting || r.Status == TransferRequestStatus.Dispatching)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<TransferRequest?> CancelIfPendingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var updated = await _context.TransferRequests
            .Where(r => r.Id == id &&
                (r.Status == TransferRequestStatus.Waiting || r.Status == TransferRequestStatus.Dispatching))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(r => r.Status, TransferRequestStatus.Cancelled)
                .SetProperty(r => r.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        if (updated == 0)
        {
            return null;
        }

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<TransferRequest>> GetExpiredSendRequestsAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .Where(r =>
                r.Type == TransferRequestType.Send &&
                r.Status == TransferRequestStatus.Waiting &&
                r.ExpiresAt != null &&
                r.ExpiresAt <= utcNow)
            .OrderBy(r => r.ExpiresAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<bool> ExistsActiveReceiveOnStationAsync(
        string toStationCode,
        CancellationToken cancellationToken = default)
    {
        return await _context.TransferRequests.AnyAsync(r =>
            (r.Status == TransferRequestStatus.Waiting || r.Status == TransferRequestStatus.Dispatching) &&
            r.Type == TransferRequestType.Receive &&
            r.ToStationCode == toStationCode,
            cancellationToken);
    }

    public async Task<bool> ExistsActiveSendOnStationAsync(
        string fromStationCode,
        CancellationToken cancellationToken = default)
    {
        return await _context.TransferRequests.AnyAsync(r =>
            (r.Status == TransferRequestStatus.Waiting || r.Status == TransferRequestStatus.Dispatching) &&
            r.Type == TransferRequestType.Send &&
            r.FromStationCode == fromStationCode,
            cancellationToken);
    }

    public async Task<IReadOnlyList<TransferRequest>> GetPendingWarehouseRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.WarehousePendingKind != WarehousePendingKind.None)
            .OrderBy(r => r.PendingWarehouseStationCode)
            .ThenBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TransferRequest>> GetWaitingReceiveForWarehouseRetryAsync(
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.Type == TransferRequestType.Receive &&
                r.WarehousePendingKind == WarehousePendingKind.None &&
                !r.IsEmptyTray)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TransferRequest>> GetWaitingEmptyTrayReceivesForWarehouseRetryAsync(
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Waiting &&
                r.Type == TransferRequestType.Receive &&
                r.WarehousePendingKind == WarehousePendingKind.None &&
                r.IsEmptyTray)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TransferRequest>> GetDispatchingOlderThanAsync(
        DateTime updatedBeforeUtc,
        CancellationToken cancellationToken = default)
    {
        var dbModels = await _context.TransferRequests
            .AsNoTracking()
            .Where(r =>
                r.Status == TransferRequestStatus.Dispatching &&
                r.UpdatedAt <= updatedBeforeUtc)
            .OrderBy(r => r.UpdatedAt)
            .ToListAsync(cancellationToken);

        return dbModels.Select(m => m.ToDomain()).ToList();
    }

    public async Task<bool> DeleteIfDispatchingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _context.TransferRequests
            .Where(r => r.Id == id && r.Status == TransferRequestStatus.Dispatching)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted > 0;
    }
}
