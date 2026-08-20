using Microsoft.EntityFrameworkCore;
using Wcs.Api.Repositories;
using Wcs.Api.Services;
using Wcs.Common.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Infrastructure.Data;

using Wcs.Rcs;
using Wcs.OpcUa;
using Wcs.Api.Configs;
using Wcs.Api.EventPublishers;
using Wcs.Api.Workers;
using Wcs.Api.EventHandlers;
using Wcs.Api.EventTransformers;
using Wcs.Api.RobotFlow;
using Wcs.Api.RobotFlow.Flow;
using Wcs.Api.RobotFlow.UseCases;
using Wcs.Api.OrderMatching;

using DotNetCore.CAP;                          
using DotNetCore.CAP.InMemoryStorage;     
using DotNetCore.CAP.RabbitMQ;
using Savorboard.CAP.InMemoryMessageQueue;
using Wcs.Infrastructure.Repositories;
using Wcs.Okamura;
using Wcs.Okamura.Services;

namespace Wcs.Api;

public static class IoC
{
    public static IServiceCollection AddIoC(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<WcsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IFlowTaskRepository, FlowTaskRepository>();
        services.AddScoped<IStationRepository, StationRepository>();
        services.AddScoped<IAmrRepository, AmrRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();
        services.AddScoped<ICraneTaskNoSequenceRepository, CraneTaskNoSequenceRepository>();
        services.AddScoped<ITransferRequestRepository, TransferRequestRepository>();
        services.AddScoped<ICassetteRepository, CassetteRepository>();

        // Services
        services.AddSingleton<IFlowTaskService, FlowTaskService>();
        services.AddSingleton<RobotService>();
        services.AddSingleton<AmrControlService>();
        services.AddSingleton<TaskExecutionGateService>();
        services.AddScoped<StationService>();
        services.AddScoped<StationSnapshotService>();
        services.AddScoped<StationControlCleanupService>();
        services.AddSingleton<IAmrLoadStateService, AmrLoadStateService>();
        services.AddScoped<DropStationRedirectService>();
        services.AddSingleton<IManualInboundService, ManualInboundService>();
        services.AddSingleton<IManualOutboundService, ManualOutboundService>();
        services.AddSingleton<IManualCraneTaskTracker, ManualCraneTaskTracker>();
        services.AddSingleton<ICraneTaskNoGenerator, CraneTaskNoGenerator>();
        services.AddSingleton<TaskScopedLockService>();
        services.AddScoped<CraneTaskDispatchService>();
        services.AddScoped<ICraneInboundQrFinishedDispatchStore, CraneInboundQrFinishedDispatchStore>();

        // WMS
        services.Configure<WmsOptions>(configuration.GetSection("Wms"));
        services.AddHttpClient<IWmsService, WmsService>((sp, client) =>
        {
            var wmsOptions = configuration.GetSection("Wms").Get<WmsOptions>();
            client.BaseAddress = new Uri(wmsOptions?.BaseUrl ?? "http://localhost:8080");
        });

        // task live event broadcaster (SSE)
        services.AddSingleton<ITaskEventBroadcaster, TaskEventBroadcaster>();

        // event publisher
        services.AddSingleton<IEventPublisher, CapEventPublisher>();

        // event transformers
        services.AddSingleton<OpcTagEventTransformer>();
        services.AddSingleton<EventTransformerRegistry>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<EventTransformerRegistry>>();
            var registry = new EventTransformerRegistry(logger);
            var opcTransformer = sp.GetRequiredService<OpcTagEventTransformer>();
            registry.Register(opcTransformer);
            return registry;
        });

        // rcs
        services.AddRcs(configuration.GetSection("Rcs"));

        // opc ua
        services.AddOpcUa(configuration.GetSection("OpcUa"));

        // Okamura
        services.AddOkamura();

        // config service
        services.AddSingleton<IConfigService>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<ConfigService>>();
            var configDir = Path.Combine(AppContext.BaseDirectory, "config");
            return new ConfigService(configDir, logger);
        });
        services.Configure<HikConfig>(configuration.GetSection("Hik"));
        services.Configure<FlowTimeoutOptions>(configuration.GetSection("FlowTimeouts"));
        services.Configure<OrderMatchingOptions>(configuration.GetSection("OrderMatching"));
        services.Configure<WarehouseOptions>(configuration.GetSection("Warehouse"));

        services.AddScoped<FlowTaskStarter>();
        services.AddScoped<TransferRequestDispatchRecoveryService>();
        services.AddScoped<TransferMatchingService>();
        services.AddScoped<AutoStationOrderRunner>();

        // event handlers
        services.AddSingleton<OpcTagChangedEventHandler>();
        services.AddSingleton<RobotFlowEventHandler>();
        services.AddSingleton<SystemEventHandler>();
        services.AddSingleton<CraneEventHandler>();
        services.AddSingleton<StationSnapshotPublisher>();

        // flow and state store
        services.AddScoped<ITaskStateStore, TaskStateStore>();
        services.AddScoped<MainFlow>();
        services.AddScoped<ErrorFlow>();
        services.AddScoped<StopFlow>();

        // use case handlers
        services.AddScoped<MoveToPickupWaitingPointHandler>();
        services.AddScoped<WarehouseOutboundOnAgvStartHandler>();
        services.AddScoped<BeforePickHandler>();
        services.AddScoped<EnterPickupPointHandler>();
        services.AddScoped<BackToPickupWaitingPointHandler>();
        services.AddScoped<MoveToDropWaitingPointHandler>();
        services.AddScoped<BeforeDropHandler>();
        services.AddScoped<EnterDropPointHandler>();
        services.AddScoped<BackToDropWaitingPointHandler>();
        services.AddScoped<EndFlowHandler>();
        services.AddScoped<LiftRackHandler>();
        services.AddScoped<PutdownRackHandler>();
        services.AddScoped<AfterPickupHandler>();
        services.AddScoped<AfterDropHandler>();

        // host
        // services.AddHostedService<EventProcessingWorker>();
        // Đăng ký OpcUaMonitorWorker như Singleton để có thể inject vào các service khác
        services.AddSingleton<OpcUaMonitorWorker>();
        services.AddHostedService(sp => sp.GetRequiredService<OpcUaMonitorWorker>());
        services.AddHostedService<FlowTaskTimeoutWorker>();
        services.AddHostedService<StationControlCleanupWorker>();
        services.AddHostedService<AutoStationOrderWorker>();
        services.AddHostedService<TransferRequestDispatcherWorker>();
        services.AddHostedService<TransferRequestDispatchRecoveryWorker>();

        services.AddCap(x => {
            x.UseSqlServer(configuration.GetConnectionString("CapConnection") ?? throw new InvalidOperationException("CapConnection not found"));
            x.EnableSubscriberParallelExecute = true;
            x.SubscriberParallelExecuteThreadCount = configuration.GetValue("Cap:SubscriberParallelExecuteThreadCount", 8);

            // Dùng hàng đợi trong bộ nhớ
            // x.UseInMemoryStorage();
            // x.UseInMemoryMessageQueue();
            // x.UseDashboard();

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
