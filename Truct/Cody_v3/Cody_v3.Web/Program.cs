using Cody_v3.Repositories.Contexts;
using Cody_v3.Repositories.Generics;
using Cody_v3.Repositories.Interfaces;
using Cody_v3.Services.DTOs;
using Cody_v3.Services.Generics;
using Cody_v3.Services.Hubs;
using Cody_v3.Services.Interfaces;
using Cody_v3.Services.ServiceClasses;
using Cody_v3.Web.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + @"\logs\Cody_v3_.txt",
    outputTemplate: "{NewLine}========================Start============================{NewLine}{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}=========================End============================={NewLine}"
    , rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Host.UseSerilog();


// Cấu hình Cookie
builder.Services.ConfigureApplicationCookie(options => {
    // options.Cookie.HttpOnly = true;  
    options.ExpireTimeSpan = TimeSpan.FromDays(5);
    options.LoginPath = $"/login/"; // Url đến trang đăng nhập
    options.LogoutPath = $"/logout/";
    options.AccessDeniedPath = $"/khongduoctruycap.html"; // Trang khi User bị cấm truy cập
});

//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient(typeof(IGenericService<>), typeof(GenericService<>));
builder.Services.AddTransient<IRecloserDataService, RecloserDataService>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<IAppDbContext>(provider => provider.GetService<AppDbContext>());
builder.Services.AddTransient<IDapperGenericRepository, DapperGenericRepository>();
builder.Services.AddTransient<IDapperGenericService, DapperGenericService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapHub<ExcelHandlingHub>("/ExcelHandlingHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
