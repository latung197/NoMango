using System.Security.Claims;
using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.Others;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Features.Erp.Organization;
using Core.Application.Interface.SysInterface;
using Core.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Features.Erp.Users;

[ApiController]
[Route("api/erp/users")]
[Authorize]
public sealed class UsersController(ISysUserService users, IAccessControlService access,
    IOrganizationService organization) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> Search([FromQuery] string? username, [FromQuery] string? email,
        [FromQuery] string? fullName, [FromQuery] int? role, [FromQuery] int? enabled)
    {
        var result = await users.SearchUser(new SysUserSearchImpl
        {
            Username = username ?? string.Empty, Email = email ?? string.Empty,
            Fullname = fullName ?? string.Empty, Role = role ?? -1, Enable = enabled ?? 1
        });
        return Ok(result);
    }

    [HttpGet("{userId:int}")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> Get(int userId) => Ok(await users.GetUserById(userId));

    [HttpPost]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> Create(SysUserDto request) => Ok(await users.InsertUser(request));

    [HttpPut("{userId:int}")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> Update(int userId, SysUserDto request)
    {
        if (request.UserId != userId) return BadRequest(new { message = "Mã tài khoản không khớp." });
        return Ok(await users.UpdateUser(request));
    }

    [HttpDelete("{userId:int}")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> Delete(int userId) => Ok(await users.DeleteUser(userId));

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeMyPassword(ChangePassword request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        request.UserId = userId;
        return Ok(await users.ChangePassword(request));
    }

    [HttpGet("{userId:int}/access")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> GetAccess(int userId) => Ok(await access.GetUserAccessAsync(userId));

    [HttpPut("{userId:int}/roles")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> SetRoles(int userId, SetUserRolesRequest request, CancellationToken ct)
    {
        try { await access.SetUserRolesAsync(userId, request.RoleIds, ct); return NoContent(); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{userId:int}/permissions")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> SetPermissions(int userId, SetUserPermissionsRequest request, CancellationToken ct)
    {
        try { await access.SetUserPermissionsAsync(userId, request.Permissions, ct); return NoContent(); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{userId:int}/plants")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> GetPlants(int userId, CancellationToken ct) =>
        Ok(await organization.GetUserPlantsAsync(userId, ct));

    [HttpPut("{userId:int}/plants")]
    [Authorize(Policy = "AccessAdmin")]
    public async Task<IActionResult> SetPlants(int userId, SetUserPlantsRequest request, CancellationToken ct)
    {
        try { await organization.SetUserPlantsAsync(userId, request.PlantCodes, ct); return NoContent(); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
