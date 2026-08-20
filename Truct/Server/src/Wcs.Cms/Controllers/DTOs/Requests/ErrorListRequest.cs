using System.Text.Json.Serialization;
using Wcs.Infrastructure;

namespace Wcs.Cms.Controllers.DTOs.Requests;

//public class ErrorListRequest : QueryArgsBase {
public class ErrorListRequest {
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    [JsonPropertyName("area")]
    public string? Area { get; set; }
}