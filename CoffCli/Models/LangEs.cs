using System.Text.Json.Serialization;

namespace CoffCli.Models;

internal class LangEs
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}