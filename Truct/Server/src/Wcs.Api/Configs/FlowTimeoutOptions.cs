using System.Collections.Generic;
using System.Threading;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Configs;

public sealed class FlowTimeoutOptions
{
    public bool Enabled { get; set; } = true;
    public int DefaultSeconds { get; set; } = 300;
    public int PollingIntervalSeconds { get; set; } = 15;
    public Dictionary<string, int> Steps { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Các size task (vd. S, M, XL) không áp dụng giám sát timeout.
    /// </summary>
    public List<string> IgnoreSizes { get; set; } = [];

    /// <summary>
    /// Chỉ các step khai báo trong <see cref="Steps"/> (value &gt; 0) mới được giám sát timeout,
    /// trừ task thuộc <see cref="IgnoreSizes"/>.
    /// </summary>
    public bool IsTimeoutMonitored(FlowStep step, Size taskSize) =>
        !IsSizeIgnored(taskSize) && TryGetStepTimeoutSeconds(step, out _);

    public bool IsSizeIgnored(Size taskSize)
    {
        if (IgnoreSizes.Count == 0)
        {
            return false;
        }

        var sizeName = taskSize.ToString();
        return IgnoreSizes.Any(s => string.Equals(s, sizeName, StringComparison.OrdinalIgnoreCase));
    }

    public TimeSpan GetTimeoutForStep(FlowStep step)
    {
        if (!TryGetStepTimeoutSeconds(step, out var seconds))
        {
            return Timeout.InfiniteTimeSpan;
        }

        return TimeSpan.FromSeconds(seconds);
    }

    private bool TryGetStepTimeoutSeconds(FlowStep step, out int seconds)
    {
        seconds = 0;
        if (!Enabled || Steps.Count == 0)
        {
            return false;
        }

        var stepName = step.ToString();
        foreach (var kvp in Steps)
        {
            if (string.Equals(kvp.Key, stepName, StringComparison.OrdinalIgnoreCase) && kvp.Value > 0)
            {
                seconds = kvp.Value;
                return true;
            }
        }

        return false;
    }

    public TimeSpan GetPollingInterval()
    {
        var seconds = PollingIntervalSeconds <= 0 ? 10 : PollingIntervalSeconds;
        return TimeSpan.FromSeconds(Math.Max(5, seconds));
    }
}
