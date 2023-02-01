namespace PrismBot.SDK.Models;

public class ServerFactory
{
    public static Server CreateServer(string serverName, string host, int port, string token, string identity)
    {
        return new Server
        {
            ServerName = serverName,
            Host = host,
            Port = port,
            Token = token,
            Identity = identity
        };
    }
}