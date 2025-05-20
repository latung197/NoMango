using Microsoft.Extensions.Hosting.WindowsServices;
using PlastMB.Application.Wrapper;
using PlastMB.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using PlastMB.Application.AutoMapper;
using NLog;
using PlastMB.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.CookiePolicy;
using PlastMB.Application.Middleware;

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
//Add connect string to DBcontext
string DbType = builder.Configuration.GetConnectionString("DatabaseType");
switch (DbType)
{
    case "1"://MSSQL
        builder.Services.AddDbContext<PlastMBContext>(o => o.UseSqlServer(Environment.GetEnvironmentVariable("PlastMBContext")));
        break;
    case "2"://Postgre
        builder.Services.AddDbContext<PlastMBContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("PlastMBContext")));
        break;
    default://MSSQL
        builder.Services.AddDbContext<PlastMBContext>(o => o.UseSqlServer(Environment.GetEnvironmentVariable("PlastMBContext")));
        break;
}

//Add Cors
builder.Services.AddCors(o => o.AddPolicy("PlastMBCorsPolicy", b =>
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

    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Tokens:Issuer"],
        ValidAudience = builder.Configuration["Tokens:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"])),
        //Do not check the expiry of token
        ValidateLifetime = false
    };
});
//DI service
builder.Services.DependencyInjectionService();
//Run app as window service
builder.Host.UseWindowsService();
//Add mapping custom model - entity
PlastMBAutoMapper.Configure(builder.Services);
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
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
//Route for controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseMvc();
app.Run();