using Microsoft.Extensions.Logging;

using ModelContextProtocol.Server;

using System.ComponentModel;

namespace WeatherMcpServer.Tools;

public class WeatherTools(WeatherService weatherService, ILogger<WeatherTools> logger) : McpToolsBase (logger)
{
    [McpServerTool]
    [Description("Gets current weather conditions for the specified city.")]
    public Task<string> GetCurrentWeather(
    [Description("The city name to get weather for")] string city,
    [Description("Optional: Country code (e.g., 'US', 'UK')")] string? countryCode = null) =>
        ProcessRequest(() => weatherService.GetCurrentWeatherAsync(city, countryCode));

    [McpServerTool]
    [Description("Gets weather forecast for the specified location.")]
    public Task<string> GetWeatherForecast(
        [Description("The city name")] string city,
        [Description("Optional country code")] string? countryCode = null,
        [Description("Number of forecast days (max 5)")] int days = 5) =>
        ProcessRequest(() => weatherService.GetWeatherForecastAsync(city, countryCode, days));

    [McpServerTool]
    [Description("Gets weather alerts for the specified city.")]
    public Task<string> GetWeatherAlerts(
        [Description("The city name")] string city,
        [Description("Optional country code")] string? countryCode = null) =>
        ProcessRequest(() => weatherService.GetWeatherAlertsAsync(city, countryCode));
}
