using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Worker.Application.AutoMapper
{
    public class WorkerAutoMapper
    {
        public static void Configure(IServiceCollection services)
        {
            // .... Ignore code before this

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
