using Core.Application.AutoMapper;
using Core.Application.Middleware;
using Core.Application.Wrapper;
using Core.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region CONFIGURATION

// appsettings.json 

#endregion

#region SERVICES

// Controllers (API)
builder.Services.AddControllers();

// OpenAPI (Swagger m?i .NET 8+)
builder.Services.AddOpenApi();

// ===== DATABASE =====
string dbType = builder.Configuration.GetConnectionString("DatabaseType");

switch (dbType)
{
    case "1": // SQL Server
        builder.Services.AddDbContext<CoreContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("CoreContext")
            ));
        break;

    case "2": // PostgreSQL
        builder.Services.AddDbContext<CoreContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("CoreContext")
            ));
        break;

    default:
        throw new Exception("DatabaseType không hợp lệ");
}

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("CoreCorsPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin(); // API thư cong mở cho tất cả các domain. để kết nối từ client
    });
});

// ===== LOGGING (NLOG) =====
string appBasePath = Directory.GetCurrentDirectory();
GlobalDiagnosticsContext.Set("appbasepath", appBasePath);

LogManager.Setup()
    .LoadConfigurationFromFile(Path.Combine(appBasePath, "nlog.config"));

builder.Services.AddHttpContextAccessor();

// ===== AUTHENTICATION =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true, // kiểm tra xenh hạn token

            ValidIssuer = builder.Configuration["Tokens:Issuer"],
            ValidAudience = builder.Configuration["Tokens:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"])
            )
        };
    });

// ===== DI =====
builder.Services.DependencyInjectionService();

// ===== AUTOMAPPER =====
CoreAutoMapper.Configure(builder.Services);

#endregion

// ================= BUILD APP =================
var app = builder.Build();

#region MIDDLEWARE

// OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Global exception
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseMiddleware<ExceptionMiddleware>();

// Cookie policy 
app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest
});

// Pipeline configuration
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("CoreCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Map API
app.MapControllers();

#endregion

// Postgre legacy ( )
//AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();
