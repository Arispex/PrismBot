using System.Text.Json.Serialization;

namespace PrismBot.SDK.Models;

public class Item
{
    [JsonPropertyName("netID")] public int NetId { get; set; }
    [JsonPropertyName("prefix")] public int Prefix { get; set; }
    [JsonPropertyName("stack")] public int Stack { get; set; }
}