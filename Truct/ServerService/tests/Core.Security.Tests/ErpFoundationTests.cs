using System.Security.Claims;
using Core.Application.Features.Erp.Auth;
using Core.Application.Features.Erp.Menu;
using Core.Application.Features.Erp.Settings;
using Core.Features.Erp;
using Xunit;

namespace Core.Security.Tests;

public sealed class ErpFoundationTests
{
    [Fact]
    public void MenuTreeBuilder_NestsAndSortsChildren()
    {
        var tree = MenuTreeBuilder.Build([
            new MenuNodeDto { Code = "ROOT", Text = "Root" },
            new MenuNodeDto { Code = "SECOND", ParentCode = "ROOT", SortOrder = 2 },
            new MenuNodeDto { Code = "FIRST", ParentCode = "ROOT", SortOrder = 1 }
        ]);

        Assert.Single(tree);
        Assert.Equal(["FIRST", "SECOND"], tree[0].Children.Select(x => x.Code));
    }

    [Fact]
    public void ErpClaims_RequireBothUnitAndPlant()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, "7"),
            new Claim(ErpClaimTypes.UnitCode, "DVCS01")
        ], "test"));
        Assert.False(ErpClaims.TryGet(user, out _, out _, out _));

        user.AddIdentity(new ClaimsIdentity([new Claim(ErpClaimTypes.PlantCode, "PLANT01")]));
        Assert.True(ErpClaims.TryGet(user, out var userId, out var unit, out var plant));
        Assert.Equal(7, userId);
        Assert.Equal("DVCS01", unit);
        Assert.Equal("PLANT01", plant);
    }

    [Fact]
    public void Settings_PlantOverridesUnitAndGlobal()
    {
        SettingDto[] settings = [
            new("DATE_FORMAT", "global", "GLOBAL", true),
            new("DATE_FORMAT", "unit", "U:DVCS01", true),
            new("DATE_FORMAT", "plant", "P:PLANT01", true)
        ];
        var effective = SettingResolver.Resolve(settings, "DVCS01", "PLANT01");
        Assert.Equal("plant", Assert.Single(effective).Value);
    }
}
