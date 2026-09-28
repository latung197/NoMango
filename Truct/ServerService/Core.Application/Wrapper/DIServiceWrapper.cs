using Core.Application.Interface;
using Core.Application.Interface.SysInterface;
using Core.Application.Services;
using Core.Application.Services.SysService;
using Core.Utils.LogUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Wrapper
{
    public static class DIServiceWrapper
    {
        public static void AddCoreApplication(this IServiceCollection services)
        {
            //Inject service module when start
            services.AddSingleton<ILoggerManager, LoggerManagerImpl>();
            services.AddSingleton<Core.Application.Mapping.CoreMapper>();
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
