using CoreMVC.BL.Interfaces;
using CoreMVC.BL.Services;
using CoreMVC.DA;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILanguageService, MstLanguageService>();

CultureInfo[] cultures = { new CultureInfo("ja-JP"), new CultureInfo("vi-VN") };
builder.Services.Configure(delegate (RequestLocalizationOptions options)
{
    options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault((CultureInfo x) => x.Name == "vi-VN")?.Name ?? "vi-VN");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});
builder.Services.AddSignalR();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add services
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
