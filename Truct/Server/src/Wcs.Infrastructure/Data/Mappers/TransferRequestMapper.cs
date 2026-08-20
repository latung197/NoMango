using System.Text.Json;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;
using Wcs.Infrastructure.Data.Models;

namespace Wcs.Infrastructure.Data.Mappers;

public static class TransferRequestMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static TransferRequest ToDomain(this TransferRequestDbModel dbModel)
    {
        List<CranePosition>? cranePositions = null;
        if (!string.IsNullOrWhiteSpace(dbModel.CranePositionsJson))
        {
            cranePositions = JsonSerializer.Deserialize<List<CranePosition>>(dbModel.CranePositionsJson, JsonOptions);
        }

        return new TransferRequest
        {
            Id = dbModel.Id,
            Type = dbModel.Type,
            FromStageCode = dbModel.FromStageCode,
            ToStageCode = dbModel.ToStageCode,
            Size = dbModel.Size,
            FromStationCode = dbModel.FromStationCode,
            ToStationCode = dbModel.ToStationCode,
            IsEmptyTray = dbModel.IsEmptyTray,
            IsWipTray = dbModel.IsWipTray,
            WarehousePendingKind = dbModel.WarehousePendingKind,
            PendingWarehouseStationCode = dbModel.PendingWarehouseStationCode,
            CassetteCode = dbModel.CassetteCode,
            Product = dbModel.Product,
            Quantity = dbModel.Quantity,
            StorageStageCode = dbModel.StorageStageCode,
            RobotCode = dbModel.RobotCode,
            CranePositions = cranePositions,
            Source = dbModel.Source,
            Status = dbModel.Status,
            MatchedRequestId = dbModel.MatchedRequestId,
            FlowTaskId = dbModel.FlowTaskId,
            ExpiresAt = dbModel.ExpiresAt,
            CreatedAt = dbModel.CreatedAt,
            UpdatedAt = dbModel.UpdatedAt,
        };
    }

    public static TransferRequestDbModel ToDbModel(this TransferRequest domain)
    {
        string? cranePositionsJson = null;
        if (domain.CranePositions is { Count: > 0 })
        {
            cranePositionsJson = JsonSerializer.Serialize(domain.CranePositions, JsonOptions);
        }

        return new TransferRequestDbModel
        {
            Id = domain.Id,
            Type = domain.Type,
            FromStageCode = domain.FromStageCode,
            ToStageCode = domain.ToStageCode,
            Size = domain.Size,
            FromStationCode = domain.FromStationCode,
            ToStationCode = domain.ToStationCode,
            IsEmptyTray = domain.IsEmptyTray,
            IsWipTray = domain.IsWipTray,
            WarehousePendingKind = domain.WarehousePendingKind,
            PendingWarehouseStationCode = domain.PendingWarehouseStationCode,
            CassetteCode = domain.CassetteCode,
            Product = domain.Product,
            Quantity = domain.Quantity,
            StorageStageCode = domain.StorageStageCode,
            RobotCode = domain.RobotCode,
            CranePositionsJson = cranePositionsJson,
            Source = domain.Source,
            Status = domain.Status,
            MatchedRequestId = domain.MatchedRequestId,
            FlowTaskId = domain.FlowTaskId,
            ExpiresAt = domain.ExpiresAt,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
        };
    }
}
