using System.Text.Json;
using Microsoft.Extensions.Options;
using Wcs.Api.Configs;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data.Mappers;

namespace Wcs.Api.Services;

public sealed class TransferRequestDispatchRecoveryService(
    ITransferRequestRepository transferRequestRepository,
    IOptions<OrderMatchingOptions> options,
    ILogger<TransferRequestDispatchRecoveryService> logger)
{
    private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly ITransferRequestRepository _transferRequestRepository = transferRequestRepository;
    private readonly OrderMatchingOptions _options = options.Value;
    private readonly ILogger<TransferRequestDispatchRecoveryService> _logger = logger;

    public async Task RecoverStuckDispatchingAsync(CancellationToken ct = default)
    {
        if (!_options.DispatchingRecoveryEnabled)
        {
            return;
        }

        var threshold = DateTime.UtcNow - _options.GetDispatchingStuckTimeout();
        var stuckRequests = await _transferRequestRepository.GetDispatchingOlderThanAsync(threshold, ct);

        if (stuckRequests.Count == 0)
        {
            return;
        }

        _logger.LogWarning(
            "Found {Count} transfer request(s) stuck in Dispatching for over {TimeoutMinutes} minute(s)",
            stuckRequests.Count,
            _options.DispatchingStuckTimeoutMinutes);

        foreach (var request in stuckRequests)
        {
            await RecoverOneAsync(request, ct);
        }
    }

    private async Task RecoverOneAsync(TransferRequest request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.FlowTaskId))
        {
            _logger.LogWarning(
                "Skip deleting stuck Dispatching request {RequestId} — already linked to FlowTask {FlowTaskId}",
                request.Id,
                request.FlowTaskId);
            return;
        }

        var recoveredAt = DateTime.UtcNow;
        var stuckDurationSeconds = Math.Max(0, (int)(recoveredAt - request.UpdatedAt).TotalSeconds);
        var snapshotJson = JsonSerializer.Serialize(request.ToDbModel(), SnapshotJsonOptions);

        var deleted = await _transferRequestRepository.DeleteIfDispatchingAsync(request.Id, ct);
        if (!deleted)
        {
            _logger.LogWarning(
                "Stuck Dispatching request {RequestId} could not be deleted (status may have changed). Snapshot={RequestSnapshot}",
                request.Id,
                snapshotJson);
            return;
        }

        _logger.LogWarning(
            "Deleted stuck Dispatching request {RequestId}: {Type} {FromStage}->{ToStage} size={Size} source={Source} " +
            "fromStation={FromStation} toStation={ToStation} emptyTray={IsEmptyTray} wipTray={IsWipTray} " +
            "matchedRequestId={MatchedRequestId} cassette={CassetteCode} robot={RobotCode} " +
            "createdAt={CreatedAt:O} dispatchingSince={DispatchingSince:O} stuckSeconds={StuckSeconds} " +
            "recoveredAt={RecoveredAt:O} snapshot={RequestSnapshot}",
            request.Id,
            request.Type,
            request.FromStageCode,
            request.ToStageCode,
            request.Size,
            request.Source,
            request.FromStationCode,
            request.ToStationCode,
            request.IsEmptyTray,
            request.IsWipTray,
            request.MatchedRequestId,
            request.CassetteCode,
            request.RobotCode,
            request.CreatedAt,
            request.UpdatedAt,
            stuckDurationSeconds,
            recoveredAt,
            snapshotJson);
    }
}
