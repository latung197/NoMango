using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class StationUpdateRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("stage_code")]
    public required string StageCode { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("type")]
    public required int Type { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [MinLength(1, ErrorMessage = "Phải chọn ít nhất một size")]
    [JsonPropertyName("sizes")]
    public required int[] Sizes { get; set; }

    [JsonPropertyName("tag_has_cassette")]
    public string? TagHasCassette { get; set; }

    [JsonPropertyName("tag_status")]
    public string? TagStatus { get; set; }

    [JsonPropertyName("tag_control")]
    public string? TagControl { get; set; }

    [JsonPropertyName("tag_qrcode")]
    public string? TagQRCode { get; set; }

    [JsonPropertyName("has_curtain")]
    public bool HasCurtain { get; set; } = false;

    [JsonPropertyName("tag_curtain_state")]
    public string? TagCurtainState { get; set; }

    [JsonPropertyName("has_conveyor")]
    public bool HasConveyor { get; set; } = false;

    [JsonPropertyName("tag_conveyor_state")]
    public string? TagConveyorState { get; set; }

    [JsonPropertyName("tag_conveyor_number")]
    public string? TagConveyorNumber { get; set; }

    [JsonPropertyName("has_warehouse_door")]
    public bool HasWarehouseDoor { get; set; } = false;

    [JsonPropertyName("is_auxiliary_material")]
    public bool IsAuxiliaryMaterial { get; set; } = false;

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; } = 0;

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [MaxLength(50, ErrorMessage = "Vui lòng nhập Điểm đặt trong phạm vi tối đa 50 ký tự")]
    [JsonPropertyName("main_point")]
    public required string MainPoint { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [MaxLength(50, ErrorMessage = "Vui lòng nhập Điểm chờ trong phạm vi tối đa 50 ký tự")]
    [JsonPropertyName("waiting_point")]
    public required string WaitingPoint { get; set; }

    [JsonPropertyName("auto_receive_enabled")]
    public bool AutoReceiveEnabled { get; set; } = false;

    [JsonPropertyName("auto_send_enabled")]
    public bool AutoSendEnabled { get; set; } = false;

    [JsonPropertyName("auto_send_is_empty_tray")]
    public bool AutoSendIsEmptyTray { get; set; } = false;

    [JsonPropertyName("auto_send_to_stage")]
    public string? AutoSendToStage { get; set; }
}
