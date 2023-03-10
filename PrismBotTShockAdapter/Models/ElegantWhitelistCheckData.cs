using System.Text.Json.Serialization;

namespace PrismBotTShockAdapter.Models;

public class ElegantWhitelistCheckData
{
    [JsonPropertyName("isFreeze")] public bool IsFreeze;
    [JsonPropertyName("isRegistered")] public bool IsRegistered;

    [JsonPropertyName("playerName")] public string PlayerName;
}