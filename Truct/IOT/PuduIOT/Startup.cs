
using PuduIOT.BL;
using PuduIOT.BL.Interfaces;
using PuduIOT.BL.Services;
using PuduIOT.DA;
using PuduIOT.DA.Middleware;
using PuduIOT.Models;
using Infrastructure.SystemLog;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace PuduIOT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IMstUnitService, MstUnitService>();
            services.AddScoped<IMstRobotInfoService, MstRobotInfoService>();
            services.AddScoped<IMstConfigurationService, MstConfigurationService>();
            services.AddScoped<ILoggers, LoggerCollection>();
            services.AddScoped<ILanguageService, MstLanguageService>();
            services.AddScoped<ILibs, Libs>();
            services.AddScoped<IBreadcrumbService, BreadcrumbService>();
            services.AddScoped<IPuduApiService, PuduApiService>();
            services.AddDbContext<PuduIotDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });
          
            services.AddLocalization();
            services.AddControllersWithViews().AddViewLocalization();
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            ILanguageService languageService = serviceProvider.GetRequiredService<ILanguageService>();
            IEnumerable<MstLanguage> languages = languageService.GetAllLanguages();
           // CultureInfo[] cultures = languages.Select((mst_language x) => new CultureInfo(x.lang_code)).Distinct().ToArray();
            CultureInfo[] cultures = {new CultureInfo("ja-JP") , new CultureInfo("vi-VN")};
            services.Configure(delegate (RequestLocalizationOptions options)
            {
                options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault((CultureInfo x) => x.Name == "vi-VN")?.Name ?? "vi-VN");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });
            services.AddSignalR();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=index}/{id?}");
                endpoints.MapHub<RealTimeHub>("/realTimeHub");
                endpoints.MapHub<RealTimeRobotDetealHub>("/realTimeDetailHub");
            });
            
        }
    }
}
