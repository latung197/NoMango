using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Controllers.DTOs;

public class SendTransferRequestDto
{
    [Required]
    [JsonPropertyName("from_station")]
    public required string FromStation { get; set; }

    [Required]
    [JsonPropertyName("to_stage")]
    public required string ToStage { get; set; }

    [JsonPropertyName("robot_code")]
    public string? RobotCode { get; set; }

    [JsonPropertyName("is_empty_tray")]
    public bool IsEmptyTray { get; set; }

    [JsonPropertyName("cassette_code")]
    public string? CassetteCode { get; set; }

    [JsonPropertyName("storage_stage_code")]
    public string? StorageStageCode { get; set; }

    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    [JsonPropertyName("product")]
    public string? Product { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }
}

public class ReceiveTransferRequestDto
{
    /// <summary>Bắt buộc khi nhận hàng thường. Không cần khi <see cref="IsEmptyTray"/> hoặc <see cref="IsWipTray"/> = true.</summary>
    [JsonPropertyName("from_stage")]
    public string? FromStage { get; set; }

    [Required]
    [JsonPropertyName("to_station")]
    public required string ToStation { get; set; }

    [JsonPropertyName("robot_code")]
    public string? RobotCode { get; set; }

    [JsonPropertyName("is_empty_tray")]
    public bool IsEmptyTray { get; set; }

    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("is_wip_tray")]
    public bool IsWipTray { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }
}

public class TransferRequestResponseDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string FromStageCode { get; set; } = string.Empty;
    public string ToStageCode { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string? FromStationCode { get; set; }
    public string? ToStationCode { get; set; }
    public bool IsEmptyTray { get; set; }
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("is_wip_tray")]
    public bool IsWipTray { get; set; }

    [JsonPropertyName("warehouse_pending_kind")]
    public string WarehousePendingKind { get; set; } = "None";

    [JsonPropertyName("pending_warehouse_station_code")]
    public string? PendingWarehouseStationCode { get; set; }

    public string Status { get; set; } = string.Empty;
    public string? FlowTaskId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static TransferRequestResponseDto FromEntity(TransferRequest request) => new()
    {
        Id = request.Id,
        Type = request.Type.ToString(),
        FromStageCode = request.FromStageCode,
        ToStageCode = request.ToStageCode,
        Size = request.Size.ToString(),
        FromStationCode = request.FromStationCode,
        ToStationCode = request.ToStationCode,
        IsEmptyTray = request.IsEmptyTray,
        Source = request.Source.ToString(),
        IsWipTray = request.IsWipTray,
        WarehousePendingKind = request.WarehousePendingKind.ToString(),
        PendingWarehouseStationCode = request.PendingWarehouseStationCode,
        Status = request.Status.ToString(),
        FlowTaskId = request.FlowTaskId,
        ExpiresAt = request.ExpiresAt,
        CreatedAt = request.CreatedAt,
        UpdatedAt = request.UpdatedAt,
    };
}
