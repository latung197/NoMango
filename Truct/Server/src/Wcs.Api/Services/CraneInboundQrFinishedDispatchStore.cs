using Wcs.Okamura.Services;

namespace Wcs.Api.Services;

public sealed class CraneInboundQrFinishedDispatchStore(CraneTaskDispatchService dispatchService)
    : ICraneInboundQrFinishedDispatchStore
{
    private readonly CraneTaskDispatchService _dispatchService = dispatchService;

    public Task<int?> TryGetActiveInboundTaskNoAsync(CancellationToken ct = default)
        => _dispatchService.TryGetActiveInboundTaskNoAsync(ct);

    public Task RecordD2451RaisedAsync(int taskNo, CancellationToken ct = default)
        => _dispatchService.RecordD2451RaisedAsync(taskNo, ct);

    public Task RecordD2451ClearedAsync(int taskNo, CancellationToken ct = default)
        => _dispatchService.RecordD2451ClearedAsync(taskNo, ct);
}
