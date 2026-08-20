namespace Wcs.Okamura.Services;

/// <summary>
/// Ghi nhận chu kỳ D2451 (QR Finished) trên dispatch crane inbound theo TaskNo.
/// Implementation nằm ở Wcs.Api (DB).
/// </summary>
public interface ICraneInboundQrFinishedDispatchStore
{
    Task<int?> TryGetActiveInboundTaskNoAsync(CancellationToken ct = default);

    Task RecordD2451RaisedAsync(int taskNo, CancellationToken ct = default);

    Task RecordD2451ClearedAsync(int taskNo, CancellationToken ct = default);
}
