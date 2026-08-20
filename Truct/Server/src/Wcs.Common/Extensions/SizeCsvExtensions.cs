using Wcs.Common.ValueObjects;

namespace Wcs.Common.Extensions;

public static class SizeCsvExtensions
{
    public static List<Size> ParseSizes(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return [];
        }

        return csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => (Size)int.Parse(s))
            .Distinct()
            .OrderBy(s => (int)s)
            .ToList();
    }

    public static string ToSizesCsv(IEnumerable<Size> sizes)
    {
        return string.Join(",", sizes.Distinct().OrderBy(s => (int)s).Select(s => (int)s));
    }

    public static string NormalizeCsv(string? csv) => ToSizesCsv(ParseSizes(csv));

    public static bool ContainsSize(string? csv, Size size)
    {
        var token = $",{(int)size},";
        var normalized = $",{NormalizeCsv(csv)},";
        return normalized.Contains(token, StringComparison.Ordinal);
    }
}
