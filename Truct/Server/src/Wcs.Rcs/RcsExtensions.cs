using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wcs.Rcs.Contracts;

namespace Wcs.Rcs;

public static class RcsExtensions
{
    public static IServiceCollection AddRcs(this IServiceCollection services, IConfigurationSection section)
    {
        ArgumentNullException.ThrowIfNull(section);

        services.Configure<RcsOptions>(section);
        return services.AddRcsCore();
    }

    private static IServiceCollection AddRcsCore(this IServiceCollection services)
    {
        services.AddHttpClient<IRcsClient, RcsClient>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<RcsOptions>>().Value;
            if (string.IsNullOrWhiteSpace(opt.BaseUrl))
                throw new InvalidOperationException("RcsOptions.BaseUrl is required.");

            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}