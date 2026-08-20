using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers.DTOs.Responses;

public class MasterDataResponse
{
    public IEnumerable<StageArea> Areas { get; set; } = [];
}