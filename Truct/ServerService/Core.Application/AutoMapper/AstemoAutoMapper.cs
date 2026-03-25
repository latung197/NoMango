using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.AutoMapper
{
    public class CoreAutoMapper
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}