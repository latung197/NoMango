using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class AmrCreateRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    //[Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    [JsonPropertyName("mapCode")]
    public string? MapCode { get; set; }

    [JsonPropertyName("robotStatus")]
    public string? RobotStatus { get; set; }

    [JsonPropertyName("typeCode")]
    public string? TypeCode { get; set; }

    [JsonPropertyName("battery")]
    public string? Battery { get; set; }

    [JsonPropertyName("direction")]
    public string? Direction { get; set; }

    [JsonPropertyName("exclude")]
    public string? Exclude { get; set; }

    [JsonPropertyName("excludeStr")]
    public string? ExcludeStr { get; set; }

    [JsonPropertyName("onLine")]
    public string? OnLine { get; set; }

    [JsonPropertyName("podCode")]
    public string? PodCode { get; set; }

    [JsonPropertyName("podDir")]
    public string? PodDir { get; set; }

    [JsonPropertyName("posX")]
    public string? PosX { get; set; }

    [JsonPropertyName("posY")]
    public string? PosY { get; set; }

    [JsonPropertyName("robotIp")]
    public string? RobotIp { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("statusStr")]
    public string? StatusStr { get; set; }

    [JsonPropertyName("stop")]
    public string? Stop { get; set; }

    [JsonPropertyName("stopStr")]
    public string? StopStr { get; set; }
}
