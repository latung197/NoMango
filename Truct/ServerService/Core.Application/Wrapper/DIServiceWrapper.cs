using Core.Application.BaseHttp.Implementations;
using Core.Application.BaseHttp.Interface;
using Core.Application.Interface;
using Core.Application.Interface.SysInterface;
using Core.Application.Services;
using Core.Application.Services.SysService;
using Core.Domain.Interface;
using Core.Infrastructure.ContextAccessors;
using Core.Infrastructure.Repositories;
using Core.Utils.LogUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Wrapper
{
    public static class DIServiceWrapper
    {
        public static void DependencyInjectionService(this IServiceCollection services)
        {
            //Inject service module when start
            services.AddSingleton<ILoggerManager, LoggerManagerImpl>();
            services.AddHttpClient<IBaseHttpClient, BaseHttpClientImpl>();   //Transient, don't Inject to Scope or Singleton
            services.AddSingleton<IBaseHttpClientFactory, BaseHttpClientFactoryImpl>();
            services.AddScoped<IBaseRepositoryWrapper, BaseRepositoryWrapperImpl>();
            services.AddScoped<IUserPrincipalService, UserPrincipalService>();
            services.AddScoped<ISysUserService, SysUserServiceImpl>();
            services.AddScoped<ISysUserCommandService, SysUserCommandServiceImpl>();
            services.AddScoped<IExportPlanService, ExportPlanServiceImpl>();
            services.AddScoped<IMstDataService, MstDataServiceImpl>();
            services.AddScoped<IEcuDataService, EcuDataServiceImpl>();
            services.AddScoped<IHandyService, HandyServiceImpl>();
            services.AddScoped<IBackupService, BackupServiceImpl>();
            services.AddScoped<ITestService, TestServiceImpl>();
        }
    }
}