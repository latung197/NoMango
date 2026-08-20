namespace Wcs.Api.Configs;

using Wcs.Common.ValueObjects;

public sealed class WarehouseOptions
{
    /// <summary>
    /// Sizes allowed for warehouse operations. Empty means all sizes.
    /// Current warehouse stock is only S, so default skips noisy polling for other sizes.
    /// </summary>
    public List<string> AllowSizes { get; set; } = ["S"];

    public bool AllowsSize(Size size)
    {
        if (AllowSizes.Count == 0)
            return true;

        var sizeName = size.ToString();
        return AllowSizes.Any(s => string.Equals(s, sizeName, StringComparison.OrdinalIgnoreCase));
    }
}
