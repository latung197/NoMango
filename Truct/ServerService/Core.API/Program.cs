using Core.Application.Wrapper;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add config json file
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
//Add connect string to DBcontext
string? DbType = builder.Configuration.GetConnectionString("DatabaseType");
switch (DbType)
{
    case "1"://MSSQL
        builder.Services.AddDbContext<CoreContext>(o => o.UseSqlServer(Environment.GetEnvironmentVariable("CoreContext")));
        break;
    case "2"://Postgre
        builder.Services.AddDbContext<CoreContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("CoreContext")));
        break;
    default://MSSQL 
        builder.Services.AddDbContext<CoreContext>(o => o.UseSqlServer(Environment.GetEnvironmentVariable("CoreContext")));
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

    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Tokens:Issuer"],
        ValidAudience = builder.Configuration["Tokens:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"] ?? string.Empty)),
        //Do not check the expiry of token
        ValidateLifetime = false
    };
});
//DI service
builder.Services.DependencyInjectionService();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
