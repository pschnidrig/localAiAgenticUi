using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;

namespace LocalAgenticUi.Tools;

public sealed record WeatherReport(string City, double? TemperatureC, string Condition);

// Real data from Open-Meteo (https://open-meteo.com): free, no API key.
// The city name goes to their geocoding API, the coordinates to their forecast API.
public sealed class WeatherTool(HttpClient http)
{
    public const string Name = "get_weather";

    [Description("Gets the current weather for a city.")]
    public async Task<WeatherReport> GetWeather(
        [Description("The city name, e.g. Naters")] string city,
        CancellationToken cancellationToken = default)
    {
        var geo = await http.GetFromJsonAsync<GeocodingResponse>(
            $"https://geocoding-api.open-meteo.com/v1/search?count=1&format=json&name={Uri.EscapeDataString(city.Trim())}",
            cancellationToken);

        if (geo?.Results is not [var place, ..])
        {
            return new WeatherReport(city, null, "City not found");
        }

        var lat = place.Latitude.ToString(CultureInfo.InvariantCulture);
        var lon = place.Longitude.ToString(CultureInfo.InvariantCulture);
        var forecast = await http.GetFromJsonAsync<ForecastResponse>(
            $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,weather_code",
            cancellationToken);

        var name = place.Country is { } country ? $"{place.Name}, {country}" : place.Name;
        return new WeatherReport(
            name,
            forecast?.Current is { } current ? Math.Round(current.Temperature, 1) : null,
            forecast?.Current is { } c ? Describe(c.WeatherCode) : "Unknown");
    }

    // WMO weather interpretation codes, as documented by Open-Meteo
    private static string Describe(int code) => code switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        56 or 57 => "Freezing drizzle",
        61 or 63 or 65 => "Rain",
        66 or 67 => "Freezing rain",
        71 or 73 or 75 or 77 => "Snow",
        80 or 81 or 82 => "Rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown"
    };

    private sealed record GeocodingResponse(List<GeocodingResult>? Results);

    private sealed record GeocodingResult(string Name, string? Country, double Latitude, double Longitude);

    private sealed record ForecastResponse(CurrentWeather? Current);

    private sealed record CurrentWeather(
        [property: JsonPropertyName("temperature_2m")] double Temperature,
        [property: JsonPropertyName("weather_code")] int WeatherCode);
}
