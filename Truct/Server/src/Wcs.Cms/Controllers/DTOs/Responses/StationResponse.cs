using System.Text.Json.Serialization;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class StationResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("stage_code")]
    public string StageCode { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("sizes")]
    public int[] Sizes { get; set; } = [];
    
    [JsonPropertyName("tag_has_cassette")]
    public string? TagHasCassette { get; set; }
    
    [JsonPropertyName("tag_status")]
    public string? TagStatus { get; set; }
    
    [JsonPropertyName("tag_control")]
    public string? TagControl { get; set; }

    [JsonPropertyName("tag_qrcode")]
    public string? TagQRCode { get; set; }

    [JsonPropertyName("has_curtain")]
    public bool HasCurtain { get; set; }
    
    [JsonPropertyName("tag_curtain_state")]
    public string? TagCurtainState { get; set; }
    
    [JsonPropertyName("has_conveyor")]
    public bool HasConveyor { get; set; }
    
    [JsonPropertyName("tag_conveyor_state")]
    public string? TagConveyorState { get; set; }
    
    [JsonPropertyName("tag_conveyor_number")]
    public string? TagConveyorNumber { get; set; }

    [JsonPropertyName("has_warehouse_door")]
    public bool HasWarehouseDoor { get; set; }

    [JsonPropertyName("is_auxiliary_material")]
    public bool IsAuxiliaryMaterial { get; set; }

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
    
    [JsonPropertyName("main_point")]
    public string MainPoint { get; set; } = string.Empty;
    
    [JsonPropertyName("waiting_point")]
    public string WaitingPoint { get; set; } = string.Empty;

    [JsonPropertyName("auto_receive_enabled")]
    public bool AutoReceiveEnabled { get; set; }

    [JsonPropertyName("auto_send_enabled")]
    public bool AutoSendEnabled { get; set; }

    [JsonPropertyName("auto_send_is_empty_tray")]
    public bool AutoSendIsEmptyTray { get; set; }

    [JsonPropertyName("auto_send_to_stage")]
    public string? AutoSendToStage { get; set; }

    [JsonPropertyName("tag_count")]
    public int TagCount { get; set; }
    
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public static StationResponse FromStation(Station station)
    {
        // Count OPC tags
        var tagCount = 0;
        if (!string.IsNullOrEmpty(station.Tag_HasCassette)) tagCount++;
        if (!string.IsNullOrEmpty(station.Tag_Status)) tagCount++;
        if (!string.IsNullOrEmpty(station.Tag_Control)) tagCount++;
        if (!string.IsNullOrEmpty(station.Tag_QRCode)) tagCount++;
        if (!string.IsNullOrEmpty(station.Tag_CurtainState)) tagCount++;
        if (!string.IsNullOrEmpty(station.Tag_ConveyorState)) tagCount++;
        if (!string.IsNullOrEmpty(station.Tag_ConveyorNumber)) tagCount++;

        return new StationResponse
        {
            Id = station.Id,
            Code = station.Code,
            StageCode = station.StageCode,
            Type = (int)station.Type,
            Sizes = station.Sizes.Select(s => (int)s).ToArray(),
            TagHasCassette = station.Tag_HasCassette,
            TagStatus = station.Tag_Status,
            TagControl = station.Tag_Control,
            TagQRCode = station.Tag_QRCode,
            HasCurtain = station.HasCurtain,
            TagCurtainState = station.Tag_CurtainState,
            HasConveyor = station.HasConveyor,
            TagConveyorState = station.Tag_ConveyorState,
            TagConveyorNumber = station.Tag_ConveyorNumber,
            HasWarehouseDoor = station.HasWarehouseDoor,
            IsAuxiliaryMaterial = station.IsAuxiliaryMaterial,
            Capacity = station.Capacity,
            MainPoint = station.MainPoint,
            WaitingPoint = station.WaitingPoint,
            AutoReceiveEnabled = station.AutoReceiveEnabled,
            AutoSendEnabled = station.AutoSendEnabled,
            AutoSendIsEmptyTray = station.AutoSendIsEmptyTray,
            AutoSendToStage = station.AutoSendToStage,
            TagCount = tagCount,
            IsActive = station.IsActive,
            CreatedAt = station.CreatedAt,
            UpdatedAt = station.UpdatedAt
        };
    }
}

