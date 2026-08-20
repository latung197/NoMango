namespace Wcs.Okamura.Models;

public record OpcTagFailure(
    string Tag,
    string Description,
    string Expected,
    string Actual);

public record OpcConditionCheckResult(
    bool IsSatisfied,
    IReadOnlyList<OpcTagFailure> Failures)
{
    public static OpcConditionCheckResult Ok { get; } = new(true, []);

    public static OpcConditionCheckResult FromFailures(IReadOnlyList<OpcTagFailure> failures)
        => failures.Count == 0 ? Ok : new(false, failures);

    public List<string> ToErrorMessages()
        => Failures
            .Select(f => $"{f.Tag} ({f.Description}): yêu cầu={f.Expected}, hiện tại={f.Actual}")
            .ToList();
}

public sealed class OpcConditionsNotMetException : Exception
{
    public OpcConditionCheckResult Result { get; }

    public OpcConditionsNotMetException(OpcConditionCheckResult result)
        : base(FormatMessage(result))
    {
        Result = result;
    }

    private static string FormatMessage(OpcConditionCheckResult result)
        => result.Failures.Count == 0
            ? "Điều kiện OPC chưa đáp ứng"
            : "Điều kiện OPC chưa đáp ứng: " + string.Join("; ", result.ToErrorMessages());
}
