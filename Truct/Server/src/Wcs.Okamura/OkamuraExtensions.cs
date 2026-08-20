using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wcs.Okamura.Services;

namespace Wcs.Okamura;

public static class OkamuraExtensions
{
    public static IServiceCollection AddOkamura(this IServiceCollection services)
    {
        services.AddSingleton<ICraneService, CraneService>();
        services.AddSingleton<IWarehouseQrCodeStore, WarehouseQrCodeStore>();
        services.AddSingleton<IStationHandshakeService, StationHandshakeService>();
        services.AddSingleton<ICraneMonitorService, CraneMonitorService>();

        return services;
    }    
}