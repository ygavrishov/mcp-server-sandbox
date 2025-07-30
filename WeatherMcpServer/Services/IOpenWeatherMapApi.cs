using Refit;

public interface IOpenWeatherMapApi
{
    /// <summary>
    /// Gets current weather
    /// </summary>
    /// <param name="q">City, countryCode (for example, "London,UK")</param>
    /// <param name="appid">API key</param>
    /// <returns></returns>
    [Get("/data/2.5/weather")]
    Task<string> GetWeatherAsync(
        string q,
        string appid,
        string units = "metric");

    // 5 day forecast
    [Get("/data/2.5/forecast")]
    Task<string> GetWeatherForecastAsync(
        string q,
        string appid,
        string units = "metric",
        int cnt = 40); // forecast point count (max 40)

    /// <summary>
    /// Gets weather Forecast (ATTENTION: One Call API 3.0 subscription is necessary!)
    /// </summary>
    [Get("/data/3.0/onecall")]
    Task<string> GetWeatherAlertsAsync(
        double lat,
        double lon,
        string appid,
        string units = "metric",
        string exclude = "minutely,hourly,daily"); // Exclude unnecessary data

    /// <summary>
    /// Geocoding: city → coordinates
    /// </summary>
    /// <param name="q">City, countryCode (for example, "London,UK")</param>
    /// <param name="limit">Result item count</param>
    /// <param name="appid">API key</param>
    /// <returns></returns>
    [Get("/geo/1.0/direct")]
    Task<string> GeocodeAsync(
        string q,
        int limit,
        string appid
    );
}
