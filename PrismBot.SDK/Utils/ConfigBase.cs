using YamlDotNet.Serialization;

namespace PrismBot.SDK.Utils;

public abstract class ConfigBase<T> where T : ConfigBase<T>, new()
{
    protected virtual string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, typeof(T).Name + ".yml");

    private static T? _instance;
    public static T Instance => _instance ?? Load();

    #region Load

    /// <summary>
    /// 由 默认路径 加载 默认配置文件
    /// </summary>
    /// <returns>默认配置文件实例</returns>
    public static T Load() =>
        _instance = LoadFrom(new T().ConfigFilePath);

    /// <summary>
    /// 由 指定路径 加载 配置文件 (不保存至<see cref="Instance"/>)
    /// </summary>
    /// <param name="filePath">配置文件路径</param>
    /// <returns>配置文件实例</returns>
    public static T LoadFrom(string filePath)
    {
        if (File.Exists(filePath))
            return new Deserializer().Deserialize<T>(File.ReadAllText(filePath));

        var config = new T();
        File.WriteAllText(filePath, new Serializer().Serialize(config));
        return config;
    }

    #endregion

    #region Save

    /// <summary>
    /// 保存 默认配置文件实例 至 默认路径
    /// </summary>
    public static void Save() =>
        SaveTo(Instance, Instance.ConfigFilePath);

    /// <summary>
    /// 保存 指定配置文件实例 至 指定路径
    /// </summary>
    /// <param name="config">配置文件实例</param>
    /// <param name="filePath">指定路径</param>
    public static void SaveTo(T config, string filePath) =>
        File.WriteAllText(filePath, new Serializer().Serialize(config));

    /// <summary>
    /// 当配置文件不存在时，创建配置文件
    /// </summary>
    /// <returns>是否创建了新配置文件</returns>
    public static bool CreateIfNotExist()
    {
        var @default = new T();
        if (File.Exists(@default.ConfigFilePath))
            return false;

        SaveTo(@default, @default.ConfigFilePath);
        return true;
    }
    #endregion
}