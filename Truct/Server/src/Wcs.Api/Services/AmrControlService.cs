using Wcs.Api.Controllers.DTOs;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Rcs.Contracts;
using Wcs.Rcs.DTOs;

namespace Wcs.Api.Services;

public sealed class AmrControlService(
    IServiceScopeFactory scopeFactory,
    IRcsClient rcsClient,
    ILogger<AmrControlService> logger)
{
    public const string AmrGlobalStoppedKey = "AmrGlobalStopped";

    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IRcsClient _rcsClient = rcsClient;
    private readonly ILogger<AmrControlService> _logger = logger;

    public async Task<AmrControlStatusDto> GetStatusAsync(CancellationToken ct = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var settingRepository = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
        var value = await settingRepository.GetValueAsync(AmrGlobalStoppedKey, ct);
        return new AmrControlStatusDto(ParseStopped(value));
    }

    /// <summary>
    /// Gửi lệnh stopRobot tới RCS cho tất cả AMR đang active — dừng ngay lập tức.
    /// </summary>
    public async Task<StopAllAmrResultDto> StopAllImmediateAsync(string? reason = null, CancellationToken ct = default)
    {
        var robotCodes = await GetActiveRobotCodesAsync(ct);
        if (robotCodes.Count == 0)
        {
            _logger.LogWarning("StopAllImmediate: không có AMR active nào trong hệ thống");
            return new StopAllAmrResultDto(0, 0, []);
        }

        _logger.LogWarning(
            "StopAllImmediate: gửi stopRobot cho {Count} AMR. Reason={Reason}",
            robotCodes.Count,
            reason ?? "(none)");

        var rcsResult = await _rcsClient.StopRobot(
            new StopRobotRequest(robotCodes.Count.ToString(), robotCodes),
            ct);

        LogBulkResult("StopAllImmediate", "stopRobot", robotCodes.Count, rcsResult);

        if (rcsResult.Success)
        {
            await SetStoppedAsync(true, ct);
        }

        return MapStopResult(robotCodes, rcsResult.Success, rcsResult.Code, rcsResult.Message);
    }

    /// <summary>
    /// Gửi lệnh resumeRobot tới RCS cho tất cả AMR đang active.
    /// </summary>
    public async Task<ResumeAllAmrResultDto> ResumeAllAsync(string? reason = null, CancellationToken ct = default)
    {
        var robotCodes = await GetActiveRobotCodesAsync(ct);
        if (robotCodes.Count == 0)
        {
            _logger.LogWarning("ResumeAll: không có AMR active nào trong hệ thống");
            return new ResumeAllAmrResultDto(0, 0, []);
        }

        _logger.LogInformation(
            "ResumeAll: gửi resumeRobot cho {Count} AMR. Reason={Reason}",
            robotCodes.Count,
            reason ?? "(none)");

        var rcsResult = await _rcsClient.ResumeRobot(
            new ResumeRobotRequest(robotCodes.Count.ToString(), robotCodes),
            ct);

        LogBulkResult("ResumeAll", "resumeRobot", robotCodes.Count, rcsResult);

        if (rcsResult.Success)
        {
            await SetStoppedAsync(false, ct);
        }

        return MapResumeResult(robotCodes, rcsResult.Success, rcsResult.Code, rcsResult.Message);
    }

    private async Task SetStoppedAsync(bool stopped, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var settingRepository = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
        await settingRepository.SetValueAsync(AmrGlobalStoppedKey, stopped ? "true" : "false", ct);
    }

    private static bool ParseStopped(string? value) =>
        string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1";

    private async Task<IList<string>> GetActiveRobotCodesAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var amrRepository = scope.ServiceProvider.GetRequiredService<IAmrRepository>();
        return (await amrRepository.GetListAsync(ct)).Select(a => a.Code).ToList();
    }

    private void LogBulkResult<T>(string operation, string rcsMethod, int count, RcsResult<T> rcsResult)
    {
        if (rcsResult.Success)
        {
            _logger.LogInformation("{Operation}: {RcsMethod} thành công cho {Count} AMR", operation, rcsMethod, count);
        }
        else
        {
            _logger.LogError(
                "{Operation}: {RcsMethod} thất bại — code={Code}, message={Message}",
                operation,
                rcsMethod,
                rcsResult.Code,
                rcsResult.Message);
        }
    }

    private static StopAllAmrResultDto MapStopResult(
        IList<string> robotCodes,
        bool success,
        string code,
        string message)
    {
        var results = robotCodes.Select(robotCode => new AmrRobotControlResultDto(
            robotCode,
            success,
            code,
            message)).ToList();

        var affectedCount = success ? robotCodes.Count : 0;
        var failedCount = success ? 0 : robotCodes.Count;

        return new StopAllAmrResultDto(affectedCount, failedCount, results);
    }

    private static ResumeAllAmrResultDto MapResumeResult(
        IList<string> robotCodes,
        bool success,
        string code,
        string message)
    {
        var results = robotCodes.Select(robotCode => new AmrRobotControlResultDto(
            robotCode,
            success,
            code,
            message)).ToList();

        var affectedCount = success ? robotCodes.Count : 0;
        var failedCount = success ? 0 : robotCodes.Count;

        return new ResumeAllAmrResultDto(affectedCount, failedCount, results);
    }
}
