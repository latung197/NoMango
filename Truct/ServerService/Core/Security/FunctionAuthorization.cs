using System.Security.Claims;
using Core.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Core.Security;

public sealed class FunctionRequirement : IAuthorizationRequirement;
public sealed class AdminRequirement : IAuthorizationRequirement;

public sealed class FunctionAuthorizationHandler(IAccessControlService access) :
    AuthorizationHandler<FunctionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        FunctionRequirement requirement)
    {
        if (!int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return;
        var endpoint = (context.Resource as HttpContext)?.GetEndpoint();
        var action = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (action is null) return;

        var permission = ResolvePermission(action.ControllerName, action.ActionName,
            (context.Resource as HttpContext)?.Request.Method ?? string.Empty);
        if (permission is null) return;
        if (await access.HasPermissionAsync(userId, permission.Value.Menu, permission.Value.Action))
            context.Succeed(requirement);
    }

    public static (string Menu, string Action)? ResolvePermission(string controller, string action, string verb)
    {
        var menu = controller switch
        {
            "SysUser" => "USER",
            "ExportPlan" => "EXPORT_PLAN",
            "EcuData" => "ECU_DATA",
            "MstData" => "MASTER_DATA",
            "Backup" => "BACKUP",
            "Handy" => "HANDY",
            _ => null
        };
        if (menu is null) return null;

        var name = action.ToLowerInvariant();
        var permission = controller == "Backup" && name == "database" ? "A"
            : name.StartsWith("search") ? "S"
            : name.StartsWith("print") ? "P"
            : name.StartsWith("reload") ? "L"
            : name.StartsWith("copy") ? "Y"
            : name.StartsWith("approve") ? "A"
            : name.StartsWith("get") || name.StartsWith("check") ? "R"
            : name.StartsWith("delete") || verb == "DELETE" ? "D"
            : name.StartsWith("update") || name.StartsWith("change") ? "U"
            : name.StartsWith("import") ? "I"
            : name.StartsWith("export") ? "E"
            : name.StartsWith("insert") || name.StartsWith("add") || verb == "POST" ? "C"
            : verb == "GET" ? "R" : null;
        return permission is null ? null : (menu, permission);
    }
}

public sealed class AdminAuthorizationHandler(IAccessControlService access) :
    AuthorizationHandler<AdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        if (int.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            && await access.IsAdminAsync(userId)) context.Succeed(requirement);
    }
}
