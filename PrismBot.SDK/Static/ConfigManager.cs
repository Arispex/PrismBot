using PrismBot.SDK.Models;
using YamlDotNet.Serialization;

namespace PrismBot.SDK.Static;

public static class ConfigManager
{
    public static Dictionary<string, object>? GetConfig(string fileName)
    {
        var configPath = Path.Combine(Environment.CurrentDirectory, fileName);
        if (!File.Exists(configPath)) return null;

        using (var streamReader = new StreamReader(configPath))
        {
            var deserializer = new Deserializer();
            return deserializer.Deserialize<Dictionary<string, object>>(streamReader);
        }
    }

    public static Config? GetBotConfig()
    {
        var configPath = Path.Combine(Environment.CurrentDirectory, "config.yml");
        if (!File.Exists(configPath)) return null;
        using (var streamReader = new StreamReader(configPath))
        {
            var deserializer = new Deserializer();
            return deserializer.Deserialize<Config>(streamReader);
        }
    }
}