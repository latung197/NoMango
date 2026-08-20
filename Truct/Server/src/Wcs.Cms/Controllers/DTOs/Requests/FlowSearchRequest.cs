using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class FlowSearchRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("area")]
    public required string Area { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("stage")]
    public required string Stage { get; set; }
}
