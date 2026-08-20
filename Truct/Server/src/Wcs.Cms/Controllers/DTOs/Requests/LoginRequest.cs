using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class LoginRequest
{
    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Giá trị không được để trống")]
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}
