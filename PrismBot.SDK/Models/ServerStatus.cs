using System.Text.Json.Serialization;

namespace PrismBot.SDK.Models;

public class ServerStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("serverversion")]
    public string ServerVersion { get; set; }

    [JsonPropertyName("tshockversion")]
    public string TshockVersion { get; set; }

    [JsonPropertyName("port")]
    public long Port { get; set; }

    [JsonPropertyName("playercount")]
    public long PlayerCount { get; set; }

    [JsonPropertyName("maxplayers")]
    public long MaxPlayers { get; set; }

    [JsonPropertyName("world")]
    public string World { get; set; }

    [JsonPropertyName("uptime")]
    public string Uptime { get; set; }

    [JsonPropertyName("serverpassword")]
    public bool ServerPassword { get; set; }

    [JsonPropertyName("players")]
    public TShockPlayer[] Players { get; set; }
}