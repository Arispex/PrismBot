using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PrismBot.SDK.Models;

public class Server
{
    public string ServerName { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Token { get; set; }

    [Key] public string Identity { get; set; }

    public Server(string serverName, string host, int port, string token, string identity)
    {
        ServerName = serverName;
        Host = host;
        Port = port;
        Token = token;
        Identity = identity;
    }
    
    private Server() {}
    
    public async Task<string[]> GetActiveListAsync()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"http://{Host}:{Port}/v2/users/activelist?token={Token}");
        response.EnsureSuccessStatusCode();
        var result =
            await JsonSerializer.DeserializeAsync<Dictionary<string, String>>(
                await response.Content.ReadAsStreamAsync());
        var activeUsers = result["activeusers"];
        if (activeUsers.Length == 0) return Array.Empty<String>();
        return activeUsers.Split("\t");
    }
}