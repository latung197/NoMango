using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class ErrorUpdateRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("status")]
    public required int Status { get; set; }

    //[Required(ErrorMessage = "Giá trị không được để trống")]
    //[JsonPropertyName("area")]
    //public required string Area { get; set; }
}
