using PrismBot.SDK.Models;
using YamlDotNet.Serialization;

namespace PrismBot.SDK.Static;

public static class ConfigManager
{
    /// <summary>
    ///     获得指定配置文件
    /// </summary>
    /// <param name="paths">配置文件路径</param>
    /// <returns></returns>
    public static T GetConfig<T>(params string[] paths)
    {
        var configPath = Path.Combine(Environment.CurrentDirectory, Path.Combine(paths));
        if (!File.Exists(configPath)) throw new FileNotFoundException($"找不到配置文件 {paths}");

        using var streamReader = new StreamReader(configPath);
        var deserializer = new Deserializer();
        return deserializer.Deserialize<T>(streamReader);
    }

    public static void SaveConfig(string fileName, object obj)
    {
        var configPath = Path.Combine(Environment.CurrentDirectory, "configs", fileName);
        
        using var streamWriter = new StreamWriter(configPath);
        var serializer = new Serializer();
        serializer.Serialize(streamWriter, obj);
    }

    /// <summary>
    ///     获得机器人的主配置文件(config.yml)
    /// </summary>
    /// <returns>配置文件</returns>
    public static Config GetBotConfig()
    {
        var configPath = Path.Combine(Environment.CurrentDirectory, "config.yml");
        if (!File.Exists(configPath)) throw new FileNotFoundException("找不到配置文件 config.yml");
        using (var streamReader = new StreamReader(configPath))
        {
            var deserializer = new Deserializer();
            return deserializer.Deserialize<Config>(streamReader);
        }
    }
}