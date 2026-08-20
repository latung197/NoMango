using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Common.Extensions;

public static class StationSizeResolver
{
    private static readonly Dictionary<string, Size> LabelToSize = new(StringComparer.OrdinalIgnoreCase)
    {
        ["S"] = Size.S,
        ["M"] = Size.M,
        ["L"] = Size.L,
        ["XL"] = Size.XL,
        ["LL"] = Size.LL,
    };

    public static Size ResolveForStation(Station station, int? clientSize)
    {
        if (clientSize.HasValue)
        {
            var size = (Size)clientSize.Value;
            if (!Enum.IsDefined(typeof(Size), size))
            {
                throw new InvalidOperationException($"Size không hợp lệ: {clientSize.Value}");
            }

            if (!station.SupportsSize(size))
            {
                throw new InvalidOperationException(
                    $"Trạm {station.Code} không hỗ trợ size {size}");
            }

            return size;
        }

        if (station.Sizes.Count == 1)
        {
            return station.Sizes[0];
        }

        throw new InvalidOperationException("SIZE_REQUIRED");
    }

    public static bool TryParseCassetteSize(string? cassetteSize, out Size size)
    {
        size = default;
        if (string.IsNullOrWhiteSpace(cassetteSize))
        {
            return false;
        }

        var normalized = cassetteSize.Trim().ToUpperInvariant();
        if (LabelToSize.TryGetValue(normalized, out size))
        {
            return true;
        }

        if (int.TryParse(normalized, out var intValue) && Enum.IsDefined(typeof(Size), intValue))
        {
            size = (Size)intValue;
            return true;
        }

        return Enum.TryParse<Size>(normalized, true, out size);
    }
}
