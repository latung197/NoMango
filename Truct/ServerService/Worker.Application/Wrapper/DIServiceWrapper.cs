using Worker.Application.BaseHttp.Implementations;
using Worker.Application.BaseHttp.Interface;
using Worker.Application.Interface;
using Worker.Application.Services;
using Core.Utils.LogUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Worker.Application.Wrapper
{
    public static class DIServiceWrapper
    {
        public static void DependencyInjectionService(this IServiceCollection services)
        {
            //Inject service module when start
            services.AddHttpClient<IBaseHttpClient, BaseHttpClientImpl>();   //Transient, don't Inject to Scope or Singleton
            services.AddSingleton<IBaseHttpClientFactory, BaseHttpClientFactoryImpl>();
            services.AddSingleton<ILoggerManager, LoggerManagerImpl>();
            services.AddSingleton<IWorkerServiceClient, WorkerServiceClientImpl>();//service use BaseHttpClientFactory must be Singleton
            services.AddSingleton<IWorkerService, WorkerServiceImpl>();
        }
    }
}