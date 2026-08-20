namespace Wcs.Okamura;

using System;
using System.Threading;
using System.Threading.Tasks;

public static class OpcWaiter
{
    public static async Task WaitUntilAsync(
        Func<CancellationToken, Task<bool>> condition,
        TimeSpan timeout,
        TimeSpan pollInterval,
        CancellationToken ct)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);

        while (true)
        {
            cts.Token.ThrowIfCancellationRequested();

            if (await condition(cts.Token))
                return;

            await Task.Delay(pollInterval, cts.Token);
        }
    }
}
