using Microsoft.Extensions.Configuration;

namespace WeatherMcpServer.Configuration;

public interface IWeatherConfig
{
    string OpenWeatherMapApiKey { get; }
}

internal class WeatherConfig(IConfiguration configuration) : IWeatherConfig
{
    public string OpenWeatherMapApiKey => configuration["OpenWeatherMapApiKey"] ?? throw new ArgumentNullException(nameof(OpenWeatherMapApiKey));
}
