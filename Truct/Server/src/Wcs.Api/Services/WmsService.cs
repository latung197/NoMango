using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Wcs.Api.Configs;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Services;

public sealed class WmsApiException(
    string path,
    int statusCode,
    string? reasonPhrase,
    string responseBody)
    : Exception($"WMS call failed: {path} returned {statusCode} {reasonPhrase}")
{
    public string Path { get; } = path;
    public int StatusCode { get; } = statusCode;
    public string? ReasonPhrase { get; } = reasonPhrase;
    public string ResponseBody { get; } = responseBody;
}

public class WmsService(HttpClient httpClient, IOptions<WmsOptions> options, ILogger<WmsService> logger) : IWmsService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly WmsOptions _options = options.Value;
    private readonly ILogger<WmsService> _logger = logger;

    public async Task ConfirmPositionInboundAsync(
        string position,
        string stageCode,
        string cassetteId,
        string size,
        int quantity,
        string product,
        CancellationToken ct = default)
    {
        var payload = new
        {
            position,
            stageCode,
            cassetteId,
            size,
            quantity,
            product
        };

        await PostAsync("/api/warehouse/confirm-position-inbound", payload, ct);
    }

    public async Task CompleteTransferInboundAsync(string position, CancellationToken ct = default)
    {
        await PostAsync("/api/warehouse/complete-transfer-inbound", new { position }, ct);
    }

    public async Task<IReadOnlyList<CranePosition>?> TryFindEmptyInboundPositionsAsync(CancellationToken ct = default)
    {
        try
        {
            var data = await FetchEmptyInboundPositionDataAsync(ct);
            var positions = ToCranePositions(data);
            if (positions.Count == 0)
            {
                _logger.LogWarning("WMS has no empty inbound storage positions");
                return null;
            }

            _logger.LogInformation(
                "WMS empty inbound positions found: Count={Count}",
                positions.Count);

            return positions;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WMS unavailable or failed when checking empty inbound positions");
            return null;
        }
    }

    public async Task<IReadOnlyList<CranePosition>> FindEmptyInboundPositionsAsync(CancellationToken ct = default)
    {
        try
        {
            var data = await FetchEmptyInboundPositionDataAsync(ct);
            var positions = ToCranePositions(data);
            if (positions.Count == 0)
            {
                throw new InvalidOperationException("WMS did not return any inbound storage positions.");
            }

            _logger.LogInformation(
                "WMS empty inbound positions found: Count={Count}",
                positions.Count);

            return positions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WMS call failed: find empty inbound positions");
            throw;
        }
    }

    public async Task<string> FindReplacementInboundPositionAsync(
        string failedPosition,
        CancellationToken ct = default)
    {
        try
        {
            var data = await FetchEmptyInboundPositionDataAsync(ct);
            var position = ResolveReplacementInboundPosition(data, failedPosition);

            _logger.LogInformation(
                "WMS replacement position found: Failed={FailedPosition}, Replacement={ReplacementPosition}",
                failedPosition,
                position);

            return position;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WMS call failed: find replacement inbound position for {FailedPosition}", failedPosition);
            throw;
        }
    }

    public async Task HandleErrorInboundAsync(string failedPosition, string replacementPosition, CancellationToken ct = default)
    {
        await PostAsync("/api/warehouse/handle-error-inbound", new
        {
            failedPosition,
            replacementPosition
        }, ct);
    }

    public Task UpdatePositionStatusAsync(string position, int positionStatus, CancellationToken ct = default)
        => PatchAsync("/api/warehouse/positions/status", new
        {
            position,
            positionStatus
        }, ct);

    public async Task ConfirmPositionOutboundAsync(string position, CancellationToken ct = default)
    {
        await PostAsync("/api/warehouse/confirm-position-outbound", new { position }, ct);
    }

    public async Task CompleteTransferOutboundAsync(string position, CancellationToken ct = default)
    {
        await PostAsync("/api/warehouse/complete-transfer-outbound", new { position }, ct);
    }

    public async Task<WarehouseOutboundPosition?> FindFifoOutboundPositionAsync(
        string stageCode,
        CancellationToken ct = default)
    {
        try
        {
            var query = BuildPathWithQuery("/api/warehouse/cassettes/fifo", ("stageCode", stageCode));
            return await FetchOutboundPositionAsync(query, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WMS fifo outbound lookup failed for stage {StageCode}", stageCode);
            return null;
        }
    }

    public async Task<WarehouseOutboundPosition?> FindEmptyTrayOutboundPositionAsync(
        CancellationToken ct = default)
    {
        try
        {
            var query = BuildPathWithQuery("/api/warehouse/find-position-outbound");
            return await FetchOutboundPositionAsync(query, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WMS empty tray outbound lookup failed");
            return null;
        }
    }

    private static string BuildPathWithQuery(string path, params (string Key, string? Value)[] parameters)
    {
        var queryParts = parameters
            .Where(p => !string.IsNullOrWhiteSpace(p.Value))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}")
            .ToList();

        return queryParts.Count == 0 ? path : $"{path}?{string.Join("&", queryParts)}";
    }

    private async Task<WarehouseOutboundPosition?> FetchOutboundPositionAsync(string query, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync(query, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning(
                "WMS outbound lookup failed: {Query} returned {StatusCode} {Body}",
                query, (int)response.StatusCode, body);
            return null;
        }

        var envelope = await response.Content.ReadFromJsonAsync<WmsResponse<FindEmptyPositionData>>(cancellationToken: ct);
        var data = envelope?.Data;
        if (data is null || data.Row <= 0 || data.Floor <= 0 || data.PositionNumber <= 0)
        {
            return null;
        }

        return new WarehouseOutboundPosition
        {
            Row = data.Row,
            Floor = data.Floor,
            PositionNumber = data.PositionNumber,
        };
    }

    private async Task PatchAsync<T>(string path, T payload, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync(path, payload, ct);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(ct);
                var requestPayload = JsonSerializer.Serialize(payload);

                _logger.LogError(
                    "WMS call failed. Path={Path}, StatusCode={StatusCode}, Reason={Reason}, ResponseBody={ResponseBody}, RequestPayload={RequestPayload}",
                    path,
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    responseBody,
                    requestPayload);

                throw new WmsApiException(
                    path,
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    responseBody);
            }

            _logger.LogInformation("WMS call succeeded: {Path}", path);
        }
        catch (WmsApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var requestPayload = JsonSerializer.Serialize(payload);
            _logger.LogError(ex, "WMS call failed before receiving a valid response. Path={Path}, RequestPayload={RequestPayload}", path, requestPayload);
            throw;
        }
    }

    private async Task PostAsync<T>(string path, T payload, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(path, payload, ct);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(ct);
                var requestPayload = JsonSerializer.Serialize(payload);

                _logger.LogError(
                    "WMS call failed. Path={Path}, StatusCode={StatusCode}, Reason={Reason}, ResponseBody={ResponseBody}, RequestPayload={RequestPayload}",
                    path,
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    responseBody,
                    requestPayload);

                throw new WmsApiException(
                    path,
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    responseBody);
            }

            _logger.LogInformation("WMS call succeeded: {Path}", path);
        }
        catch (WmsApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var requestPayload = JsonSerializer.Serialize(payload);
            _logger.LogError(ex, "WMS call failed before receiving a valid response. Path={Path}, RequestPayload={RequestPayload}", path, requestPayload);
            throw;
        }
    }

    private async Task<FindEmptyPositionData> FetchEmptyInboundPositionDataAsync(CancellationToken ct)
    {
        var response = await _httpClient.GetAsync("/api/warehouse/find-empty-positions-inbound", ct);
        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<WmsResponse<FindEmptyPositionData>>(cancellationToken: ct);
        return envelope?.Data
            ?? throw new InvalidOperationException("WMS did not return inbound position data.");
    }

    private static List<CranePosition> ToCranePositions(FindEmptyPositionData data)
    {
        var positions = new List<CranePosition>();

        if (!string.IsNullOrWhiteSpace(data.Position))
        {
            positions.Add(new CranePosition(data.Row, data.Floor, data.PositionNumber));
        }

        foreach (var sub in data.SubPositions)
        {
            if (!string.IsNullOrWhiteSpace(sub.Position))
            {
                positions.Add(new CranePosition(sub.Row, sub.Floor, sub.PositionNumber));
            }
        }

        return positions;
    }

    private static string ResolveReplacementInboundPosition(FindEmptyPositionData data, string failedPosition)
    {
        var failed = failedPosition.Trim();

        if (!string.IsNullOrWhiteSpace(data.Position)
            && !string.Equals(data.Position.Trim(), failed, StringComparison.OrdinalIgnoreCase))
        {
            return data.Position.Trim();
        }

        var backupPosition = data.SubPositions?
            .Select(sub => sub.Position?.Trim())
            .FirstOrDefault(position =>
                !string.IsNullOrWhiteSpace(position)
                && !string.Equals(position, failed, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(backupPosition))
        {
            return backupPosition;
        }

        throw new InvalidOperationException(
            $"WMS did not return a replacement inbound position different from failed position {failedPosition}.");
    }

    private sealed class WmsResponse<T>
    {
        public T? Data { get; set; }
    }

    private sealed class FindEmptyPositionData
    {
        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("floor")]
        public int Floor { get; set; }

        [JsonPropertyName("row")]
        public int Row { get; set; }

        [JsonPropertyName("positionNumber")]
        public int PositionNumber { get; set; }

        [JsonPropertyName("stageCode")]
        public string? StageCode { get; set; }

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("subPositions")]
        public List<FindEmptySubPositionData> SubPositions { get; set; } = [];
    }

    private sealed class FindEmptySubPositionData
    {
        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("floor")]
        public int Floor { get; set; }

        [JsonPropertyName("row")]
        public int Row { get; set; }

        [JsonPropertyName("positionNumber")]
        public int PositionNumber { get; set; }

        [JsonPropertyName("stageCode")]
        public string? StageCode { get; set; }

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("positionStatus")]
        public int PositionStatus { get; set; }
    }
}
