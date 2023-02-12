using PrismBot.SDK.Models;
using YamlDotNet.Serialization;

namespace PrismBot.SDK.Static;

public static class ConfigManager
{
    /// <summary>
    /// 获得指定配置文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns></returns>
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

    /// <summary>
    /// 获得机器人的主配置文件(config.yml)
    /// </summary>
    /// <returns>配置文件</returns>
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