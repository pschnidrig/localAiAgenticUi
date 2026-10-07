namespace LocalAgenticUi.Configuration;

public sealed class LmStudioOptions
{
    public const string SectionName = "LmStudio";

    public string Endpoint { get; init; } = string.Empty;
    public string ModelName { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
}
