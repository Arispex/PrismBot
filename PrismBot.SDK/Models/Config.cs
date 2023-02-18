namespace PrismBot.SDK.Models;

/// <summary>
/// 机器人主配置文件对象
/// </summary>
public class Config
{
    public string AccessToken = string.Empty;
    public long[] BlockUsers = Array.Empty<long>();
    public long[] Groups = Array.Empty<long>();
    public string Host = "127.0.0.1";
    public ushort Port = 8080;
    public long[] SuperUsers = Array.Empty<long>();
    public string UniversalPath = string.Empty;
    public ushort GenHttpPort = 8081;
}