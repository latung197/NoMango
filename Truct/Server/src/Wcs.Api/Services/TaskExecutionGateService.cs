using Wcs.Api.Controllers.DTOs;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Events;

namespace Wcs.Api.Services;

/// <summary>
/// Cổng tạm dừng start task mới: task đang chạy tiếp tục, task chờ chỉ start khi resume.
/// </summary>
public sealed class TaskExecutionGateService(
    IServiceScopeFactory scopeFactory,
    IEventPublisher publisher,
    ILogger<TaskExecutionGateService> logger)
{
    public const string TaskExecutionPausedKey = "TaskExecutionPaused";

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IEventPublisher _publisher = publisher;
    private readonly ILogger<TaskExecutionGateService> _logger = logger;

    public async Task<TaskExecutionStatusDto> GetStatusAsync(CancellationToken ct = default)
    {
        return new TaskExecutionStatusDto(await IsPausedAsync(ct));
    }

    public async Task<bool> IsPausedAsync(CancellationToken ct = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var settingRepository = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
        var value = await settingRepository.GetValueAsync(TaskExecutionPausedKey, ct);
        return ParseBool(value);
    }

    public async Task PauseAsync(string? reason = null, CancellationToken ct = default)
    {
        await SetPausedAsync(true, ct);
        _logger.LogWarning("Task execution paused. Reason={Reason}", reason ?? "(none)");
    }

    public async Task<TaskExecutionResumeResultDto> ResumeAsync(string? reason = null, CancellationToken ct = default)
    {
        await SetPausedAsync(false, ct);
        _logger.LogInformation("Task execution resumed. Reason={Reason}", reason ?? "(none)");

        await using var scope = _scopeFactory.CreateAsyncScope();
        var flowTaskRepository = scope.ServiceProvider.GetRequiredService<IFlowTaskRepository>();
        var pendingTasks = (await flowTaskRepository.GetPendingStartTasksAsync(ct)).ToList();

        var startedTaskIds = new List<string>(pendingTasks.Count);
        foreach (var task in pendingTasks)
        {
            await _publisher.PublishAsync(new FlowStarted(task.Id), ct);
            startedTaskIds.Add(task.Id);
            _logger.LogInformation("Published FlowStarted for pending task {TaskId}", task.Id);
        }

        return new TaskExecutionResumeResultDto(startedTaskIds.Count, startedTaskIds);
    }

    private async Task SetPausedAsync(bool paused, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var settingRepository = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
        await settingRepository.SetValueAsync(TaskExecutionPausedKey, paused ? "true" : "false", ct);
    }

    private static bool ParseBool(string? value) =>
        string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";
}
