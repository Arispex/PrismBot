using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Web;
using PrismBot.SDK.Exceptions;

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

    public async Task<Dictionary<string, string>> SendGetToEndpointAsync(string endpointPath,
        Dictionary<string, object> @params)
    {
        var query = HttpUtility.ParseQueryString(String.Empty);
        foreach (var key in @params.Keys)
        {
            foreach (var value in @params.Values)
            {
                query[key] = value.ToString();
            }
        }
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"http://{Host}:{Port}/{endpointPath}?{query}");
        switch (response.StatusCode)
        {
            case HttpStatusCode.Forbidden:
                throw new InvalidToken("提供了无效的Token");
            case HttpStatusCode.Unauthorized:
                throw new NotAuthorized("API需要Token");
            case HttpStatusCode.BadRequest:
                throw new MissingParameters("缺少参数");
        }

        response.EnsureSuccessStatusCode();
        var result =
            await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(
                await response.Content.ReadAsStreamAsync());
        return result;
    }
    /// <summary>
    /// 获取在线且已登入的玩家昵称
    /// </summary>
    /// <returns>在线且已登入的玩家昵称的数组</returns>
    public async Task<string[]> GetActiveListAsync()
    {
        var result = await SendGetToEndpointAsync("v2/users/activelist", new Dictionary<string, object>
        {
            {"token", Token}
        });
        var activeUsers = result["activeusers"];
        if (activeUsers.Length == 0) return Array.Empty<String>();
        return activeUsers.Split("\t");
    }
}