using Core.WorkerService;
using Microsoft.Extensions.Hosting.WindowsServices;
using NLog;
using Worker.Application.Wrapper;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
        ? AppContext.BaseDirectory
        : Directory.GetCurrentDirectory()
});

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

builder.Services.AddWindowsService();

var contentRoot = builder.Environment.ContentRootPath;
GlobalDiagnosticsContext.Set("appbasepath", contentRoot);
LogManager.Setup().LoadConfigurationFromFile(Path.Combine(contentRoot, "nlog.config"));

builder.Services.AddHostedService<WorkerLine3>();
builder.Services.AddHostedService<WorkerLine4>();
builder.Services.DependencyInjectionService();

await builder.Build().RunAsync();
