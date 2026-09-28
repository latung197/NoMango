using Core.Application.Security;
using Core.Domain.Entity.SystemEntities;
using Core.Infrastructure.Security;
using Core.Infrastructure.Context;
using Core.Security;
using Core.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Core.Security.Tests;

public sealed class AccessControlTests
{
    [Fact]
    public void IndividualPermissionAddsToGroupPermission()
    {
        var effective = new PermissionGrantDto { MenuId0 = "EXPORT_PLAN", CanView = true };
        effective.Add(new PermissionGrantDto { MenuId0 = "EXPORT_PLAN", CanAdd = true });

        Assert.True(effective.Has("R"));
        Assert.True(effective.Has("C"));
        Assert.False(effective.Has("D"));
    }

    [Fact]
    public void ExistingPasswordCanBeVerifiedAndUpgraded()
    {
        var passwords = new PasswordService();
        var user = new SysUser { PasswordHash = StringUtils.Encrypt("old-password") };

        Assert.True(passwords.Verify(user, "old-password", out var needsUpgrade));
        Assert.True(needsUpgrade);
        Assert.False(passwords.Verify(user, "wrong-password", out _));

        user.PasswordHash = passwords.Hash(user, "old-password");
        Assert.NotEqual(StringUtils.Encrypt("old-password"), user.PasswordHash);
        Assert.True(passwords.Verify(user, "old-password", out needsUpgrade));
        Assert.False(needsUpgrade);
        Assert.False(passwords.Verify(user, "wrong-password", out _));
    }

    [Fact]
    public void AccessControlJoinTablesHaveCompositeKeys()
    {
        var options = new DbContextOptionsBuilder<CoreContext>()
            .UseNpgsql("Host=localhost;Database=model_only;Username=model_only;Password=model_only")
            .Options;
        using var context = new CoreContext(options);

        Assert.Equal(2, context.Model.FindEntityType(typeof(SysUserRole))!.FindPrimaryKey()!.Properties.Count);
        Assert.Equal(2, context.Model.FindEntityType(typeof(SysRoleCommand))!.FindPrimaryKey()!.Properties.Count);
        Assert.Equal(2, context.Model.FindEntityType(typeof(SysUserCommand))!.FindPrimaryKey()!.Properties.Count);
    }

    [Theory]
    [InlineData("ExportPlan", "SearchExportPlan", "POST", "EXPORT_PLAN", "S")]
    [InlineData("ExportPlan", "InsertExportPlan", "POST", "EXPORT_PLAN", "C")]
    [InlineData("EcuData", "ImportListEcuData", "POST", "ECU_DATA", "I")]
    [InlineData("Backup", "Database", "GET", "BACKUP", "A")]
    [InlineData("Handy", "CheckHandyInfo", "POST", "HANDY", "R")]
    public void EndpointMapsToExpectedFunctionAction(string controller, string action,
        string verb, string menu, string permission)
    {
        Assert.Equal((menu, permission), FunctionAuthorizationHandler.ResolvePermission(controller, action, verb));
    }
}
