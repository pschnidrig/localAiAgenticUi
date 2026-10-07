using System.ComponentModel;

namespace LocalAgenticUi.Tools;

public sealed record WeatherReport(string City, int TemperatureC, string Condition);

// Fake data on purpose: the demo is about how the tool call is rendered,
// not about calling a real weather API. Everything stays on your machine.
public static class WeatherTool
{
    public const string Name = "get_weather";

    private static readonly string[] Conditions = ["Sunny", "Cloudy", "Rain", "Snow", "Fog"];

    [Description("Gets the current weather for a city.")]
    public static WeatherReport GetWeather([Description("The city name, e.g. Zurich")] string city)
    {
        // Stable per city and day, so asking twice gives the same answer.
        // (string.GetHashCode is randomized per process, so hash by hand.)
        var key = $"{city.Trim().ToLowerInvariant()}|{DateTime.Today:yyyy-MM-dd}";
        var seed = key.Aggregate(17, (hash, c) => unchecked(hash * 31 + c));
        var random = new Random(seed);
        return new WeatherReport(city, random.Next(-5, 31), Conditions[random.Next(Conditions.Length)]);
    }
}
