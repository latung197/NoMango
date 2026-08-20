using Microsoft.EntityFrameworkCore;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Repositories;
using Wcs.Cms.Services;
using Wcs.Cms.Middlewares;
using Wcs.Cms.EventHandlers;

namespace Wcs.Cms;

public static class IoC {
    public static IServiceCollection AddIoC(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<WcsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddTransient<GlobalExceptionMiddleware>();

        // Event handlers
        services.AddSingleton<AllEventHandler>();

        // Repositories
        services.AddScoped<IStageRepository, StageRepository>();
        services.AddScoped<IStationRepository, StationRepository>();
        services.AddScoped<IFlowRepository, FlowRepository>();
        services.AddScoped<IStepRepository, StepRepository>();
        services.AddScoped<ICassetteRepository, CassetteRepository>();
        services.AddScoped<IAmrRepository, AmrRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IFlowTasksRepository, FlowTasksRepository>();
        services.AddScoped<IErrorRepository, ErrorRepository>();

        // Services
        services.AddScoped<StageService>();
        services.AddScoped<StationService>();
        services.AddScoped<FlowService>();
        services.AddScoped<StepService>();
        services.AddScoped<CassetteService>();
        services.AddScoped<AmrService>();
        services.AddScoped<MaterialService>();
        services.AddScoped<RequestService>();
        services.AddScoped<FlowTaskService>();
        services.AddScoped<ErrorService>();

        services.AddCap(x => {
            x.UseSqlServer(configuration.GetConnectionString("CapConnection") ?? throw new InvalidOperationException("CapConnection not found"));

            // Dùng hàng đợi trong bộ nhớ
            x.UseInMemoryStorage();
            // x.UseInMemoryMessageQueue();
            x.UseDashboard();

            // RabbitMQ
            x.UseRabbitMQ(x => {
                x.ConnectionFactoryOptions = cf =>
                {
                    cf.Uri = new Uri(configuration.GetConnectionString("RabbitConnection") ?? throw new InvalidOperationException("RabbitConnection not found"));
                };
            });
        });

        return services;
    }
}
