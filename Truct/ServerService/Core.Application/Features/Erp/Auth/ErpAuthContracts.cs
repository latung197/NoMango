using System.ComponentModel.DataAnnotations;
using Core.Application.Security;

namespace Core.Application.Features.Erp.Auth;

public sealed record ErpLoginOptionsRequest([Required] string Username, [Required] string Password);
public sealed record ErpLoginRequest([Required] string Username, [Required] string Password,
    [Required] string UnitCode, [Required] string PlantCode);
public sealed record ErpPlantOption(string UnitCode, string UnitName, string PlantCode, string PlantName);
public sealed record ErpLoginResult(int UserId, string UserName, string FullName, string UnitCode,
    string PlantCode, string Token, bool IsAdmin, IReadOnlyList<PermissionGrantDto> Permissions);

public interface IErpAuthService
{
    Task<IReadOnlyList<ErpPlantOption>> GetLoginOptionsAsync(ErpLoginOptionsRequest request, CancellationToken ct);
    Task<ErpLoginResult> LoginAsync(ErpLoginRequest request, CancellationToken ct);
    Task<bool> HasActiveContextAsync(int userId, string unitCode, string plantCode, CancellationToken ct);
}
