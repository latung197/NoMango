using Serilog;
using Wcs.Api;
using Wcs.Api.Middlewares;

static bool IsRunningBehindIis() =>
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_IIS_HTTPAUTH"));

var builder = WebApplication.CreateBuilder(args);

if (IsRunningBehindIis())
{
    var iisPort = Environment.GetEnvironmentVariable("ASPNETCORE_PORT");
    if (!string.IsNullOrEmpty(iisPort))
    {
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Kestrel:Endpoints:Http:Url"] = $"http://127.0.0.1:{iisPort}"
        });
    }
}

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();

    if (context.HostingEnvironment.IsDevelopment())
        loggerConfiguration.WriteTo.Console();

    var fileSection = context.Configuration.GetSection("Logging:File");
    if (!fileSection.GetValue("Enabled", true))
        return;

    var directory = fileSection["Directory"] ?? "Logs";
    var fileNameTemplate = fileSection["FileNameTemplate"] ?? "wcs-api-.log";
    var rollingIntervalStr = fileSection["RollingInterval"] ?? "Day";
    var retained = fileSection.GetValue<int?>("RetainedFileCountLimit") ?? 31;
    var shared = fileSection.GetValue("Shared", true);

    if (!Enum.TryParse<RollingInterval>(rollingIntervalStr, ignoreCase: true, out var rollingInterval))
        rollingInterval = RollingInterval.Day;

    var logsRoot = Path.IsPathRooted(directory)
        ? directory
        : Path.Combine(context.HostingEnvironment.ContentRootPath, directory);

    try
    {
        Directory.CreateDirectory(logsRoot);
    }
    catch
    {
        logsRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Wcs",
            "Logs");
        Directory.CreateDirectory(logsRoot);
    }

    var path = Path.Combine(logsRoot, fileNameTemplate);

    loggerConfiguration.WriteTo.File(
        path,
        rollingInterval: rollingInterval,
        retainedFileCountLimit: retained,
        shared: shared);
});

builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "WCS API";
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIoC(builder.Configuration);

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

    options.AddPolicy("AllowAll", corsBuilder =>
    {
        corsBuilder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Required for SignalR

        if (allowedOrigins is { Length: 1 } && allowedOrigins[0] == "*")
        {
            corsBuilder.SetIsOriginAllowed(_ => true);
        }
        else
        {
            corsBuilder.WithOrigins(allowedOrigins ?? []);
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (!IsRunningBehindIis())
    app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
