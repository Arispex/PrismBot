namespace PrismBot.SDK.Models;

public class Config
{
    public string AccessToken = string.Empty;
    public long[] BlockUsers = new long[0];
    public long[] Groups = new long[0];
    public string Host = "127.0.0.1";
    public ushort Port = 8080;
    public long[] SuperUsers = new long[0];
    public string UniversalPath = string.Empty;
    public ushort GenHttpPort = 8081;
}