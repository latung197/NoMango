using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class AmrUpdateRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("area")]
    public required string Area { get; set; }
}
