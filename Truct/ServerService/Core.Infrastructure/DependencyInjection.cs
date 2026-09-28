using Core.Application.BaseHttp.Implementations;
using Core.Application.BaseHttp.Interface;
using Core.Application.Security;
using Core.Domain.Interface;
using Core.Infrastructure.ContextAccessors;
using Core.Infrastructure.Repositories;
using Core.Infrastructure.Security;
using Core.Application.Features.Erp.Auth;
using Core.Application.Features.Erp.Organization;
using Core.Application.Features.Erp.Menu;
using Core.Application.Features.Erp.Notifications;
using Core.Application.Features.Erp.Settings;
using Core.Infrastructure.Features.Erp.Auth;
using Core.Infrastructure.Features.Erp.Organization;
using Core.Infrastructure.Features.Erp.Menu;
using Core.Infrastructure.Features.Erp.Notifications;
using Core.Infrastructure.Features.Erp.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient<IBaseHttpClient, BaseHttpClientImpl>();
        services.AddSingleton<IBaseHttpClientFactory, BaseHttpClientFactoryImpl>();
        services.AddScoped<IBaseRepositoryWrapper, BaseRepositoryWrapperImpl>();
        services.AddScoped<IUserPrincipalService, UserPrincipalService>();
        services.AddScoped<IAccessControlService, AccessControlService>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddScoped<IErpAuthService, ErpAuthService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISettingService, SettingService>();
        return services;
    }
}
