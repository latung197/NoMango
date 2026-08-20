using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

public class StationToStationTaskRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("from_station")]
    public required string FromStation { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("to_station")]
    public required string ToStation { get; set; }

    [JsonPropertyName("robot_code")]
    public string? RobotCode { get; set; }

    [JsonPropertyName("crane_positions")]
    public List<CranePositionRequest>? CranePositions { get; set; }

    [JsonPropertyName("cassette_code")]
    public string? CassetteCode { get; set; }

    /// <summary>Công đoạn lưu kho; dùng cho WMS confirm thay cho FromStation.StageCode nếu có.</summary>
    [JsonPropertyName("storage_stage_code")]
    public string? StorageStageCode { get; set; }

    /// <summary>Số lượng gửi WMS confirm inbound. Mặc định: 1</summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>Mã sản phẩm gửi WMS confirm inbound.</summary>
    [JsonPropertyName("product")]
    public string? Product { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }
}
