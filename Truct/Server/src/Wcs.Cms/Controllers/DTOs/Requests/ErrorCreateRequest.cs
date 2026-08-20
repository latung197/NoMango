using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class ErrorCreateRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("area")]
    public string? Area { get; set; }

    [JsonPropertyName("type")]
    public int ErrorType { get; set; }

    [JsonPropertyName("stage")]
    public string? Stage { get; set; }

    [JsonPropertyName("station")]
    public string? Station { get; set; }
}
