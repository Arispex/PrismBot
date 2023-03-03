using PrismBot.SDK.Utils;

namespace PrismBot.SDK.Models;

/// <summary>
///     机器人主配置文件对象
/// </summary>
public class Config : ConfigBase<Config>
{
    public string AccessToken = string.Empty;
    public long[] BlockUsers = new long[0];
    public ushort GenHttpPort = 8081;
    public long[] Groups = new long[0];
    public string Host = "127.0.0.1";
    public ushort Port = 8080;
    public long[] SuperUsers = new long[0];
    public string UniversalPath = string.Empty;
    protected override string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, "config.yml");
}