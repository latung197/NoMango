using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ILogger<AuthController> logger, IStageRepository stageRepository) : ControllerBase
{
    private readonly ILogger<AuthController> _logger = logger;
    private readonly IStageRepository _stageRepository = stageRepository;

    private static readonly string MASTER_EMAIL = "master";
    private static readonly string MASTER_PASSWORD = "nitto12345";
    private const char StageCodeSeparator = '-';

    [HttpPost("login")]
    public async Task<ActionResult<BaseResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        if (request.Email == MASTER_EMAIL && request.Password == MASTER_PASSWORD) {
            return Ok(BaseResponse<AuthResponse>.SuccessResult(new AuthResponse {
                Token = "master",
                ExpiredTime = DateTime.UtcNow.AddYears(999).ToString("yyyy-MM-ddTHH:mm:ssZ")
            }));
        }

        var stageCodes = ParseStageCodes(request.Email);
        if (stageCodes.Count > 1)
        {
            var stages = await ResolveStagesAsync(stageCodes);
            if (stages != null && ValidateMultiStagePassword(stages, request.Password))
            {
                var token = string.Join(StageCodeSeparator.ToString(), stages.Select(s => s.Code));
                return Ok(BaseResponse<AuthResponse>.SuccessResult(new AuthResponse {
                    Token = token,
                    ExpiredTime = DateTime.UtcNow.AddYears(999).ToString("yyyy-MM-ddTHH:mm:ssZ")
                }));
            }

            return BadRequest(BaseResponse<AuthResponse>.ErrorResult("Tên đăng nhập hoặc mật khẩu không chính xác"));
        }

        var station = await _stageRepository.GetByCodeAsync(request.Email);
        if (station != null && request.Password == station.Code + "12345") {
            return Ok(BaseResponse<AuthResponse>.SuccessResult(new AuthResponse {
                Token = station.Code,
                ExpiredTime = DateTime.UtcNow.AddYears(999).ToString("yyyy-MM-ddTHH:mm:ssZ")
            }));
        }
        return BadRequest(BaseResponse<AuthResponse>.ErrorResult("Tên đăng nhập hoặc mật khẩu không chính xác"));
    }

    [HttpGet("me")]
    public async Task<ActionResult<BaseResponse<ProfileResponse>>> Profile()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader) || string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return Unauthorized(BaseResponse<ProfileResponse>.ErrorResult("Thiếu header Authorization"));
        }

        var token = authorizationHeader.ToString();
        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = token["Bearer ".Length..];
        }

        if (token == "master")
        {
            var profile = new ProfileResponse
            {
                Name = "Quản trị viên",
                Email = MASTER_EMAIL,
                Role = "admin",     // Admin
                Permissions = ["master", "station"]
            };
            return Ok(BaseResponse<ProfileResponse>.SuccessResult(profile, "Lấy thông tin profile thành công"));
        }

        var stageCodes = ParseStageCodes(token);
        if (stageCodes.Count > 1)
        {
            var stages = await ResolveStagesAsync(stageCodes);
            if (stages == null)
            {
                return Unauthorized(BaseResponse<ProfileResponse>.ErrorResult("Token không hợp lệ"));
            }

            var areas = stages.Select(s => s.Area.ToString()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (areas.Count != 1)
            {
                return Unauthorized(BaseResponse<ProfileResponse>.ErrorResult("Các công đoạn phải cùng khu vực"));
            }

            var codes = stages.Select(s => s.Code).ToList();
            var profile = new ProfileResponse
            {
                Name = "Người thao tác công đoạn " + string.Join(", ", codes),
                Email = "Công đoạn " + string.Join(StageCodeSeparator, codes),
                Role = string.Join(StageCodeSeparator, codes),
                Permissions = ["station"],
                Area = areas[0],
                StageCodes = codes
            };

            return Ok(BaseResponse<ProfileResponse>.SuccessResult(profile, "Lấy thông tin profile thành công"));
        }

        var station = await _stageRepository.GetByCodeAsync(token);
        if (station != null) {
            var profile = new ProfileResponse
            {
                Name = "Người thao tác công đoạn " + station.Code,
                Email = "Công đoạn " + station.Code,
                Role = station.Code,
                Permissions = ["station"],
                Area = station.Area.ToString(),
                StageCodes = [station.Code]
            };

            return Ok(BaseResponse<ProfileResponse>.SuccessResult(profile, "Lấy thông tin profile thành công"));
        }

        return Unauthorized(BaseResponse<ProfileResponse>.ErrorResult("Token không hợp lệ"));
    }

    [HttpGet("client-ip")]
    public ActionResult<BaseResponse<ClientIpResponse>> GetClientIp()
    {
        var ip = GetClientIpAddress();
        return Ok(BaseResponse<ClientIpResponse>.SuccessResult(new ClientIpResponse { Ip = ip }, "Lấy IP client thành công"));
    }

    private static List<string> ParseStageCodes(string value) =>
        value
            .Split(StageCodeSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .ToList();

    private async Task<List<Stage>?> ResolveStagesAsync(IReadOnlyList<string> stageCodes)
    {
        var stages = new List<Stage>();

        foreach (var code in stageCodes)
        {
            var stage = await _stageRepository.GetByCodeAsync(code);
            if (stage == null)
            {
                return null;
            }

            stages.Add(stage);
        }

        return stages;
    }

    private static bool ValidateMultiStagePassword(IReadOnlyList<Stage> stages, string password)
    {
        var expectedPassword = string.Join(StageCodeSeparator, stages.Select(stage => stage.Code + "12345"));
        return password == expectedPassword;
    }

    private string GetClientIpAddress()
    {
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var remoteIp = HttpContext.Connection.RemoteIpAddress;
        if (remoteIp == null)
        {
            return string.Empty;
        }

        if (remoteIp.IsIPv4MappedToIPv6)
        {
            remoteIp = remoteIp.MapToIPv4();
        }

        return remoteIp.ToString();
    }
}
