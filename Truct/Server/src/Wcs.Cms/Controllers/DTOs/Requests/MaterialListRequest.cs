using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Wcs.Infrastructure;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class MaterialListRequest : QueryArgsBase 
{
    // Override properties với [FromQuery] để bind đúng từ query string
    [FromQuery(Name = "page")]
    public override int PageNumber { get; set; } = 1;
    
    [FromQuery(Name = "page_size")]
    public override int PageSize { get; set; } = 20;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}

