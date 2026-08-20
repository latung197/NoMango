using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

/// <summary>
/// Model để ghi giá trị boolean vào OPC UA node
/// </summary>
public class OpcUaWriteValueRequest
{
    /// <summary>
    /// Giá trị boolean cần ghi. Chỉ chấp nhận:
    /// <br/>• 0 (false)
    /// <br/>• 1 (true)
    /// </summary>
    /// <example>1</example>
    [Required(ErrorMessage = "Giá trị không được để trống")]
    //[Range(0, 1, ErrorMessage = "Giá trị chỉ được là 0 hoặc 1")]
    [JsonPropertyName("value")]
    public int Value { get; set; }
}
