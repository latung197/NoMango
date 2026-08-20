using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Controllers.DTOs;

public class CreateFallbackTaskRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("to_station")]
    public required string ToStation { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("robot_code")]
    public required string RobotCode { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("start_step")]
    public required FlowStep StartStep { get; set; }
}
