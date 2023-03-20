using System.Text.Json.Serialization;

namespace PrismBot.SDK.Models;

public class PlayerInfo
{
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("inventory")] public Item[] Inventory { get; set; }
}