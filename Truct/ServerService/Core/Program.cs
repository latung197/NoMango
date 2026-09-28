using Microsoft.Extensions.Hosting.WindowsServices;
using Core.Application.Wrapper;
using Core.Infrastructure;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NLog;
using Core.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.CookiePolicy;
using Core.Middleware;
using Core.Security;
using Microsoft.AspNetCore.Authorization;
using Core.Application.Security;
using Core.Features.Erp;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

WebApplicationOptions options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                                     ? AppContext.BaseDirectory : default
};

WebApplicationBuilder builder = WebApplication.CreateBuilder(options);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
});
//Add config json file
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().AddCommandLine(args);
//Add connect string to DBcontext
string DbType = builder.Configuration.GetConnectionString("DatabaseType");
switch (DbType)
{
    case "1"://MSSQL
        builder.Services.AddDbContext<CoreContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("CoreContext")));
        break;
    case "2"://Postgre
        builder.Services.AddDbContext<CoreContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("CoreContext")));
        break;
    default://MSSQL
        builder.Services.AddDbContext<CoreContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("CoreContext")));
        break;
}

//Add Cors
builder.Services.AddCors(o => o.AddPolicy("CoreCorsPolicy", b =>
{
    b.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .AllowCredentials();
}));
//Config log
string appBasePath = Directory.GetCurrentDirectory();
GlobalDiagnosticsContext.Set("appbasepath", appBasePath);
LogManager.Setup().LoadConfigurationFromFile(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/user/Login";
    options.AccessDeniedPath = "/Error/Error/";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;
    options.Cookie.Path = "/";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            if (!int.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                || !await context.HttpContext.RequestServices.GetRequiredService<IAccessControlService>()
                    .IsActiveAsync(userId, context.HttpContext.RequestAborted))
                context.Fail("Account is inactive.");
        }
    };

    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Tokens:Issuer"],
        ValidAudience = builder.Configuration["Tokens:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"] ?? string.Empty)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true
    };
});
//DI service
builder.Services.AddCoreApplication();
builder.Services.AddCoreInfrastructure();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("FunctionAccess", policy => policy.RequireAuthenticatedUser()
        .AddRequirements(new FunctionRequirement()));
    options.AddPolicy("AccessAdmin", policy => policy.RequireAuthenticatedUser()
        .AddRequirements(new AdminRequirement()));
    options.AddPolicy("ErpContext", policy => policy.RequireAuthenticatedUser()
        .AddRequirements(new ErpContextRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, FunctionAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ErpContextAuthorizationHandler>();
//Run app as window service
builder.Host.UseWindowsService();
WebApplication app = builder.Build();
app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMiddleware<ExceptionMiddleware>();
}
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
//Route for controller
app.MapControllers();
app.Run();
