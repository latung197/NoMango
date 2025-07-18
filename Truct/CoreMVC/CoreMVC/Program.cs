using CoreMVC.DA;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();

    try
    {
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("✅ Kết nối PostgreSQL thành công.");
        }
        else
        {
            Console.WriteLine("❌ Không thể kết nối PostgreSQL.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Lỗi kết nối PostgreSQL: " + ex.Message);
    }
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
