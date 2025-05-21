using Astemo.Application.BaseHttp.Implementations;
using Astemo.Application.BaseHttp.Interface;
using Astemo.Application.Interface;
using Astemo.Application.Services;
using Astemo.Domain.Interface;
using Astemo.Infrastructure.ContextAccessors;
using Astemo.Infrastructure.Repositories;
using Astemo.Utils.LogUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Astemo.Application.Wrapper
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
            services.AddScoped<IMstUserService, MstUserServiceImpl>();
            services.AddScoped<IExportPlanService, ExportPlanServiceImpl>();
            services.AddScoped<IMstDataService, MstDataServiceImpl>();
            services.AddScoped<IEcuDataService, EcuDataServiceImpl>();
            services.AddScoped<IHandyService, HandyServiceImpl>();
            services.AddScoped<IBackupService, BackupServiceImpl>();
            services.AddScoped<ITestService, TestServiceImpl>();
        }
    }
}