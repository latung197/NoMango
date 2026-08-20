// /src/Wcs.Infrastructure/Rcs/RcsClient.cs
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Wcs.Rcs.Contracts;
using Wcs.Rcs.DTOs;

namespace Wcs.Rcs;

public sealed class RcsClient : IRcsClient
{
    private readonly HttpClient _http;
    private readonly RcsOptions _opt;

    public RcsClient(HttpClient http, IOptions<RcsOptions> opt)
    {
        _http = http;
        _opt  = opt.Value;
    }

    // Helpers
    private string GenerateReqCode()
    {
        // Tạo ReqCode với format: WCS_YYYYMMDDHHMMSSfff_XXXX
        // WCS: prefix từ ClientCode
        // YYYYMMDDHHMMSSfff: timestamp kèm milliseconds
        // XXXX: 4 số random để đảm bảo unique
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        var random = new Random().Next(1000, 9999);
        return $"{_opt.ClientCode}_{timestamp}_{random}";
    }

    private object AddDefaultFields<TReq>(TReq request)
    {
        // Tạo một anonymous object với tất cả properties của request + reqCode, clientCode, tokenCode
        var requestType = typeof(TReq);
        var properties = requestType.GetProperties();
        
        // Tạo dictionary để chứa tất cả properties
        var propertyDict = new Dictionary<string, object?>();
        
        // Copy tất cả properties từ request gốc với tên chữ thường
        foreach (var prop in properties)
        {
            var value = prop.GetValue(request);
            var lowerCaseName = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
            
            // Convert nested objects to camelCase
            if (value != null)
            {
                value = ConvertToCamelCase(value);
            }
            
            propertyDict[lowerCaseName] = value;
        }
        
        // Thêm reqCode, clientCode, tokenCode nếu chưa có hoặc null/empty
        if (!propertyDict.ContainsKey("reqCode") || string.IsNullOrEmpty(propertyDict["reqCode"] as string))
        {
            propertyDict["reqCode"] = GenerateReqCode();
        }
        
        if (!propertyDict.ContainsKey("clientCode") || string.IsNullOrEmpty(propertyDict["clientCode"] as string))
        {
            propertyDict["clientCode"] = _opt.ClientCode;
        }
        
        if (!propertyDict.ContainsKey("tokenCode") || string.IsNullOrEmpty(propertyDict["tokenCode"] as string))
        {
            propertyDict["tokenCode"] = _opt.TokenCode;
        }

        if (!propertyDict.ContainsKey("reqTime") || string.IsNullOrEmpty(propertyDict["reqTime"] as string))
        {
            propertyDict["reqTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        return propertyDict;
    }

    private object? ConvertToCamelCase(object? obj)
    {
        if (obj == null) return obj;
        
        var objType = obj.GetType();
        
        // Handle collections
        if (obj is System.Collections.IEnumerable enumerable && objType != typeof(string))
        {
            var list = new List<object?>();
            foreach (var item in enumerable)
            {
                list.Add(ConvertToCamelCase(item));
            }
            return list;
        }
        
        // Handle records/objects
        if (objType.IsClass && objType != typeof(string))
        {
            var properties = objType.GetProperties();
            var dict = new Dictionary<string, object?>();
            
            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                var lowerCaseName = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
                
                if (value != null)
                {
                    value = ConvertToCamelCase(value);
                }
                
                dict[lowerCaseName] = value;
            }
            
            return dict;
        }
        
        // Return primitive values as-is
        return obj;
    }

    private async Task<RcsResult<TResp>> Post<TReq, TResp>(string path, TReq body, CancellationToken ct)
    {
        HttpResponseMessage? res = null;
        try
        {
            var enrichedBody = AddDefaultFields(body);
            Console.WriteLine($"RCS API Request for {path}: {System.Text.Json.JsonSerializer.Serialize(enrichedBody)}");
            var url = $"{_opt.BaseUrl}/{path}";
            res = await _http.PostAsJsonAsync(url, enrichedBody, ct);
            
            // Debug: Log raw response content
            var rawContent = await res.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"RCS API Response for {path}: {rawContent}");
            
            var payload = await res.Content.ReadFromJsonAsync<RcsEnvelope<TResp>>(cancellationToken: ct);
            if (payload is null) return new(false, "-1", "Empty response", default);
            
            // Handle error responses where data might be empty string
            TResp? data = default;
            if (payload.code == "0" && payload.data != null && payload.data is not string)
            {
                try
                {
                    // Try to deserialize data to expected type
                    var dataJson = System.Text.Json.JsonSerializer.Serialize(payload.data);
                    data = System.Text.Json.JsonSerializer.Deserialize<TResp>(dataJson);
                }
                catch (System.Text.Json.JsonException)
                {
                    // If deserialization fails, data remains default
                }
            }
            
            return new(payload.code == "0", payload.code, payload.message ?? "", data);
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine($"JSON Deserialization Error for {path}: {ex.Message}");
            if (res != null)
            {
                var responseContent = await res.Content.ReadAsStringAsync(ct);
                Console.WriteLine($"Response content: {responseContent}");
            }
            return new(false, "-1", $"JSON deserialization error: {ex.Message}", default);
        }
    }

    private async Task<RcsResult<TResp>> PostDps<TReq, TResp>(string path, TReq body, CancellationToken ct)
    {
        HttpResponseMessage? res = null;
        try
        {
            var enrichedBody = AddDefaultFields(body);
            var url = $"{_opt.DpsUrl}/{path}";
            res = await _http.PostAsJsonAsync(url, enrichedBody, ct);
            
            // Debug: Log raw response content
            var rawContent = await res.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"RCS DPS API Response for {path}: {rawContent}");
            
            var payload = await res.Content.ReadFromJsonAsync<RcsEnvelope<TResp>>(cancellationToken: ct);
            if (payload is null) return new(false, "-1", "Empty response", default);
            
            // Handle error responses where data might be empty string
            TResp? data = default;
            if (payload.code == "0" && payload.data != null && payload.data is not string)
            {
                try
                {
                    // Try to deserialize data to expected type
                    var dataJson = System.Text.Json.JsonSerializer.Serialize(payload.data);
                    data = System.Text.Json.JsonSerializer.Deserialize<TResp>(dataJson);
                }
                catch (System.Text.Json.JsonException)
                {
                    // If deserialization fails, data remains default
                }
            }
            
            return new(payload.code == "0", payload.code, payload.message ?? "", data);
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine($"JSON Deserialization Error for DPS {path}: {ex.Message}");
            if (res != null)
            {
                var responseContent = await res.Content.ReadAsStringAsync(ct);
                Console.WriteLine($"DPS Response content: {responseContent}");
            }
            return new(false, "-1", $"JSON deserialization error: {ex.Message}", default);
        }
    }

    private record RcsEnvelope<T>(string code, string? message, object? data, string? reqCode);

    // Core
    public Task<RcsResult<string>> GenAgvSchedulingTask(CreateTaskRequest req, CancellationToken ct = default)
        => Post<CreateTaskRequest, string>("genAgvSchedulingTask", req, ct);

    public Task<RcsResult<ContinueTaskResponse>> ContinueTask(ContinueTaskRequest req, CancellationToken ct = default)
        => Post<ContinueTaskRequest, ContinueTaskResponse>("continueTask", req, ct);

    public Task<RcsResult<CancelTaskResponse>> CancelTask(CancelTaskRequest req, CancellationToken ct = default)
        => Post<CancelTaskRequest, CancelTaskResponse>("cancelTask", req, ct);

    public Task<RcsResult<IList<QueryTaskStatusResponse>>> QueryTaskStatus(QueryTaskStatusRequest req, CancellationToken ct = default)
        => Post<QueryTaskStatusRequest, IList<QueryTaskStatusResponse>>("queryTaskStatus", req, ct);

    // DPS status
    public Task<RcsResult<QueryAgvStatusResponse>> QueryAgvStatus(QueryAgvStatusRequest req, CancellationToken ct = default)
        => PostDps<QueryAgvStatusRequest, QueryAgvStatusResponse>("queryAgvStatus", req, ct);

    // Optional
    public Task<RcsResult<SetTaskPriorityResponse>> SetTaskPriority(SetTaskPriorityRequest req, CancellationToken ct = default)
        => Post<SetTaskPriorityRequest, SetTaskPriorityResponse>("setTaskPriority", req, ct);

    public Task<RcsResult<BindPodAndBerthResponse>> BindPodAndBerth(BindPodAndBerthRequest req, CancellationToken ct = default)
        => Post<BindPodAndBerthRequest, BindPodAndBerthResponse>("bindPodAndBerth", req, ct);

    public Task<RcsResult<BindPodAndMatResponse>> BindPodAndMat(BindPodAndMatRequest req, CancellationToken ct = default)
        => Post<BindPodAndMatRequest, BindPodAndMatResponse>("bindPodAndMat", req, ct);

    public Task<RcsResult<BindCtnrAndBinResponse>> BindCtnrAndBin(BindCtnrAndBinRequest req, CancellationToken ct = default)
        => Post<BindCtnrAndBinRequest, BindCtnrAndBinResponse>("bindCtnrAndBin", req, ct);

    public Task<RcsResult<LockPositionResponse>> LockPosition(LockPositionRequest req, CancellationToken ct = default)
        => Post<LockPositionRequest, LockPositionResponse>("lockPosition", req, ct);

    public Task<RcsResult<SyncMapDatasResponse>> SyncMapDatas(SyncMapDatasRequest req, CancellationToken ct = default)
        => Post<SyncMapDatasRequest, SyncMapDatasResponse>("syncMapDatas", req, ct);

    public Task<RcsResult<StopRobotResponse>> StopRobot(StopRobotRequest req, CancellationToken ct = default)
        => Post<StopRobotRequest, StopRobotResponse>("stopRobot", req, ct);

    public Task<RcsResult<ResumeRobotResponse>> ResumeRobot(ResumeRobotRequest req, CancellationToken ct = default)
        => Post<ResumeRobotRequest, ResumeRobotResponse>("resumeRobot", req, ct);

    public Task<RcsResult<SetAreaStateResponse>> SetAreaState(SetAreaStateRequest req, CancellationToken ct = default)
        => Post<SetAreaStateRequest, SetAreaStateResponse>("setAreaState", req, ct);

    // Public method để generate ReqCode từ bên ngoài nếu cần
    public string GenerateRequestCode()
    {
        return GenerateReqCode();
    }
}
