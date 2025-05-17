using Astemo.WorkerService;
using Microsoft.Extensions.Hosting.WindowsServices;
using Worker.Application.Wrapper;
using Worker.Application.AutoMapper;
using NLog;

WebApplicationOptions options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                                     ? AppContext.BaseDirectory : default
};

WebApplicationBuilder builder = WebApplication.CreateBuilder(options);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMvc(option => option.EnableEndpointRouting = false);
//Add config json file
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//Config log
string appBasePath = Directory.GetCurrentDirectory();
GlobalDiagnosticsContext.Set("appbasepath", appBasePath);
LogManager.Setup().LoadConfigurationFromFile(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();
builder.Services.AddHttpContextAccessor();
//Add worker service
builder.Services.AddHostedService<WorkerLine3>();
builder.Services.AddHostedService<WorkerLine4>();
//DI service
builder.Services.DependencyInjectionService();
//Run app as window service
builder.Host.UseWindowsService();
//Add mapping custom model - entity
WorkerAutoMapper.Configure(builder.Services);
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();
app.MapRazorPages();
app.Run();
