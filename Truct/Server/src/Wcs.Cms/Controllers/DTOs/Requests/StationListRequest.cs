using Wcs.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Wcs.Cms.Controllers.DTOs.Requests;

public class StationListRequest : QueryArgsBase 
{
    // Override properties với [FromQuery] để bind đúng từ query string
    [FromQuery(Name = "page")]
    public override int PageNumber { get; set; } = 1;
    
    [FromQuery(Name = "page_size")]
    public override int PageSize { get; set; } = 20;

    [FromQuery(Name = "code")]
    public string? Code { get; set; }

    [FromQuery(Name = "stage_code")]
    public string? StageCode { get; set; }

    [FromQuery(Name = "size")]
    public string? Size { get; set; }
}

