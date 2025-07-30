using Newtonsoft.Json;

using Refit;

using WeatherMcpServer.Configuration;

public class WeatherService
{
    private readonly IOpenWeatherMapApi _weatherApi;
    private readonly string _apiKey;

    public WeatherService(IWeatherConfig config)
    {
        _apiKey = config.OpenWeatherMapApiKey;
        _weatherApi = RestService.For<IOpenWeatherMapApi>("https://api.openweathermap.org");
    }

    public async Task<string> GetCurrentWeatherAsync(string city, string? countryCode = null)
    {
        try
        {
            var location = countryCode != null ? $"{city},{countryCode}" : city;
            var response = await _weatherApi.GetWeatherAsync(location, _apiKey);

            dynamic data = JsonConvert.DeserializeObject(response)!;
            return $"Weather в {city}: {data.weather[0].description}, temperature: {data.main.temp}°C";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Gets weather forecast for a city
    /// </summary>
    /// <param name="city">Target city</param>
    /// <param name="countryCode">Target country 2-letter code (optional).</param>
    /// <param name="days">How may days to foresee (should be less or equals than 5 days because of API limitation).</param>
    /// <returns></returns>
    public async Task<string> GetWeatherForecastAsync(string city, string? countryCode = null, int days = 5)
    {
        var location = FormatLocation(city, countryCode);
        var response = await _weatherApi.GetWeatherForecastAsync(location, _apiKey, cnt: days * 8); // 8 points per day
        return FormatForecastResponse(response, $"Weather forecast in {city} for {days} days");
    }

    public async Task<string> GetWeatherAlertsAsync(string city, string? countryCode = null)
    {
        // extract latitude and longitude with city name and country code
        var location = FormatLocation(city, countryCode);
        var geocodeResponse = await _weatherApi.GeocodeAsync(location, 1, _apiKey);
        var geocodeData = JsonConvert.DeserializeObject<dynamic[]>(geocodeResponse)!;
        if (geocodeData.Length == 0)
            return "City was not found";

        double lat = geocodeData[0].lat;
        double lon = geocodeData[0].lon;

        // take alerts by coordinates
        var alertsResponse = await _weatherApi.GetWeatherAlertsAsync(lat, lon, _apiKey);
        return FormatAlertsResponse(alertsResponse);
    }

    private string FormatLocation(string city, string? countryCode) => countryCode != null ? $"{city},{countryCode}" : city;

    private string FormatWeatherResponse(string jsonResponse, string title)
    {
        dynamic data = JsonConvert.DeserializeObject(jsonResponse)!;
        return $"""
            {title}
            - Description: {data.weather[0].description}
            - Temperature: {data.main.temp}°C
            - Humidity: {data.main.humidity}%
            """;
    }

    private string FormatForecastResponse(string jsonResponse, string title)
    {
        dynamic data = JsonConvert.DeserializeObject(jsonResponse)!;
        var forecast = new StringBuilder(title + "\n");

        foreach (var item in data.list)
        {
            forecast.AppendLine($"""
                - {item.dt_txt}: 
                  Temperature: {item.main.temp}°C
                  Weather: {item.weather[0].description}
                """);
        }

        return forecast.ToString();
    }

    private string FormatAlertsResponse(string jsonResponse)
    {
        dynamic data = JsonConvert.DeserializeObject(jsonResponse)!;
        if (data.alerts == null)
            return "There are no active alerts";

        var alerts = new StringBuilder("⚠️ Weather alerts:\n");
        foreach (var alert in data.alerts)
        {
            alerts.AppendLine($"""
                - Event: {alert.@event}
                - Description: {alert.description}
                - Period: from {alert.start} to {alert.end}
                """);
        }

        return alerts.ToString();
    }
}