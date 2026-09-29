using System.Text.Json.Serialization;

namespace CoffCli.Models;

internal class CurrentCondition
{
    public string? FeelsLikeC { get; set; }

    [JsonPropertyName("humidity")]
    public string? Humidity { get; set; }

    [JsonPropertyName("winddir16Point")]
    public string? Winddir16Point { get; set; }

    [JsonPropertyName("windspeedKmph")]
    public string? WindspeedKmph { get; set; }

    [JsonPropertyName("lang_es")]
    public List<LangEs>? LangEs { get; set; }
}