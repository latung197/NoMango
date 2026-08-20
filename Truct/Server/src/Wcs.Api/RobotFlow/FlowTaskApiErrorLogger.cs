using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Okamura.Models;
using Wcs.Rcs.Contracts;
using Wcs.Rcs.DTOs;

namespace Wcs.Api.RobotFlow;

public static class FlowTaskApiErrorLogger
{
    public const string EventName = "ExternalApiError";
    private const int MaxLength = 500;

    public static Task LogRcsAsync(
        ITaskStateStore store,
        FlowTask task,
        string operation,
        string code,
        string? message,
        CancellationToken ct = default)
        => store.LogAsync(task, Format("RCS", $"{operation} thất bại (code={code}): {message ?? "không có chi tiết"}"), EventName);

    public static Task LogWmsAsync(
        ITaskStateStore store,
        FlowTask task,
        string operation,
        string detail,
        CancellationToken ct = default)
        => store.LogAsync(task, Format("WMS", $"{operation} thất bại: {detail}"), EventName);

    public static Task LogWmsAsync(
        ITaskStateStore store,
        FlowTask task,
        string operation,
        WmsApiException ex,
        CancellationToken ct = default)
        => LogWmsAsync(
            store,
            task,
            operation,
            $"HTTP {ex.StatusCode} {ex.ReasonPhrase}, body={Truncate(ex.ResponseBody)}",
            ct);

    public static Task LogCraneAsync(
        ITaskStateStore store,
        FlowTask task,
        string operation,
        CraneTaskResult result,
        CancellationToken ct = default)
        => store.LogAsync(
            task,
            Format("Crane", $"{operation} thất bại (ErrorCode={result.ErrorCode}): {result.ErrorMessage ?? "không có chi tiết"}"),
            EventName);

    public static Task LogCraneAsync(
        ITaskStateStore store,
        FlowTask task,
        string operation,
        Exception ex,
        CancellationToken ct = default)
        => store.LogAsync(task, Format("Crane", $"{operation} thất bại: {ex.Message}"), EventName);

    private static string Format(string source, string detail) => Truncate($"[{source}] {detail}");

    private static string Truncate(string value) =>
        value.Length <= MaxLength ? value : value[..(MaxLength - 3)] + "...";
}

public static class FlowTaskRcsExtensions
{
    public static async Task ContinueTaskOrThrowAsync(
        this IRcsClient rcs,
        ITaskStateStore store,
        FlowTask task,
        ContinueTaskRequest request,
        CancellationToken ct = default)
    {
        var result = await rcs.ContinueTask(request, ct);
        if (!result.Success)
        {
            await FlowTaskApiErrorLogger.LogRcsAsync(store, task, "ContinueTask", result.Code, result.Message, ct);
            throw new InvalidDataException($"RCS ContinueTask thất bại (code={result.Code}): {result.Message}");
        }
    }

    public static async Task<string> GenAgvSchedulingTaskOrThrowAsync(
        this IRcsClient rcs,
        ITaskStateStore store,
        FlowTask task,
        CreateTaskRequest request,
        CancellationToken ct = default)
    {
        var result = await rcs.GenAgvSchedulingTask(request, ct);
        if (!result.Success)
        {
            await FlowTaskApiErrorLogger.LogRcsAsync(store, task, "GenAgvSchedulingTask", result.Code, result.Message, ct);
            throw new InvalidDataException($"RCS GenAgvSchedulingTask thất bại (code={result.Code}): {result.Message}");
        }

        return result.Data ?? throw new InvalidDataException("RCS GenAgvSchedulingTask không trả về task code");
    }
}
