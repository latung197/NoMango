using PlastMB.Application.BaseHttp.Implementations;
using PlastMB.Application.BaseHttp.Interface;
using PlastMB.Application.Interface;
using PlastMB.Application.Services;
using PlastMB.Domain.Interface;
using PlastMB.Infrastructure.ContextAccessors;
using PlastMB.Infrastructure.Repositories;
using PlastMB.Utils.LogUtils;
using Microsoft.Extensions.DependencyInjection;

namespace PlastMB.Application.Wrapper
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
            services.AddScoped<IBackupService, BackupServiceImpl>();
            services.AddScoped<ITestService, TestServiceImpl>();
            services.AddScoped<IMstMachineService, MstMachineServiceImpl>();
            services.AddScoped<IMstFactoryService, MstFactoryServiceImpl>();
            services.AddScoped<ITrnImportHistoryService, TrnImportHistoryServiceImpl>();
            services.AddScoped<ITrnOperationOeeService, TrnOperationOeeServiceImpl>();
            services.AddScoped<ITrnOperationResultService, TrnOperationResultServiceImpl>();
            services.AddScoped<ITrnImportHistoryDetailService, TrnImportHistoryDetailServiceImpl>();




        }
    }
}