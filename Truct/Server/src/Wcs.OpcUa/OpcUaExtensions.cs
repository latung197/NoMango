using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wcs.OpcUa.Contracts;

namespace Wcs.OpcUa;

public static class OpcUaExtensions
{
    public static IServiceCollection AddOpcUa(this IServiceCollection services, IConfigurationSection section)
    {
        ArgumentNullException.ThrowIfNull(section);

        services.Configure<OpcUaOptions>(section);

        var opt = services.BuildServiceProvider().GetRequiredService<IOptions<OpcUaOptions>>().Value;
            if (string.IsNullOrWhiteSpace(opt.EndpointUrl))
                throw new InvalidOperationException("OpcUaOptions.EndpointUrl is required.");

        services.AddSingleton<IOpcUaClient, OpcUaClient>();

        return services;
    }    
}