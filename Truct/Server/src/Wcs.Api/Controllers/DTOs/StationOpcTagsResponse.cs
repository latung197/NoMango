using System.Text.Json.Serialization;

namespace Wcs.Api.Controllers.DTOs;

public class StationOpcTagsResponse
{
    [JsonPropertyName("stationCode")]
    public string StationCode { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<StationOpcTagItem> Tags { get; set; } = [];
}

public class StationOpcTagItem
{
    [JsonPropertyName("tagType")]
    public string TagType { get; set; } = string.Empty;

    [JsonPropertyName("tagName")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("isGood")]
    public bool IsGood { get; set; }
}
