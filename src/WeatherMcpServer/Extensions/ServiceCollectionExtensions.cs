using Microsoft.Extensions.DependencyInjection;

using WeatherMcpServer.Configuration;
using WeatherMcpServer.Tools;

namespace WeatherMcpServer.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services) =>
        services
            .AddSingleton<IWeatherConfig, WeatherConfig>()
            .AddSingleton<WeatherService>()
            .AddSingleton<WeatherTools>()
            .AddSingleton<RandomNumberTools>();
}
