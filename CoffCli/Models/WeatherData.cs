using System.Text.Json.Serialization;

namespace CoffCli.Models;

internal sealed class WeatherData
{
    [JsonPropertyName("current_condition")]
    public List<CurrentCondition>? CurrentCondition { get; set; }
}