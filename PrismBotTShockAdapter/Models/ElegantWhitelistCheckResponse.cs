using System.Text.Json.Serialization;

namespace PrismBotTShockAdapter.Models;

public class ElegantWhitelistCheckResponse
{
    [JsonPropertyName("status")] public string Status { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("data")] public ElegantWhitelistCheckData Data { get; set; }
}