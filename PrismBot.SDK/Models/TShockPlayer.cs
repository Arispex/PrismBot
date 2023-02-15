using System.Text.Json.Serialization;

namespace PrismBot.SDK.Models;

public class TShockPlayer
{
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("group")]
    public string Group { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("state")]
    public long State { get; set; }

    [JsonPropertyName("team")]
    public long Team { get; set; }
}