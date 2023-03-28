using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Web;
using PrismBot.SDK.Exceptions;

namespace PrismBot.SDK.Models;

public class Server
{
    public Server(string serverName, string host, int port, string token, string identity)
    {
        ServerName = serverName;
        Host = host;
        Port = port;
        Token = token;
        Identity = identity;
    }

    private Server()
    {
    }

    public string ServerName { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Token { get; set; }

    [Key] public string Identity { get; set; }

    /// <summary>
    ///     发送GET请求至指定端点
    /// </summary>
    /// <param name="endpointPath">端点路径</param>
    /// <param name="params">GET请求参数</param>
    /// <typeparam name="T">JSON反序列化类型</typeparam>
    /// <returns>JSON反序列化结果</returns>
    /// <exception cref="InvalidTokenException">Token错误</exception>
    /// <exception cref="NotAuthorizedException">该端点需要Token验证</exception>
    /// <exception cref="MissingParametersException">缺少指定参数</exception>
    /// <exception cref="EndpointNotFoundException">端点不存在</exception>
    public async Task<T> SendGetToEndpointAsync<T>(string endpointPath,
        Dictionary<string, object> @params)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var (key, value) in @params) query[key] = value.ToString();
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"http://{Host}:{Port}/{endpointPath}?{query}");
        switch (response.StatusCode)
        {
            case HttpStatusCode.Forbidden:
                throw new InvalidTokenException("提供了无效的Token");
            case HttpStatusCode.Unauthorized:
                throw new NotAuthorizedException("API需要Token");
            case HttpStatusCode.BadRequest:
                throw new MissingParametersException("缺少参数");
            case HttpStatusCode.NotFound:
                throw new EndpointNotFoundException("API端点不存在");
        }

        response.EnsureSuccessStatusCode();
        var result =
            await JsonSerializer.DeserializeAsync<T>(
                await response.Content.ReadAsStreamAsync());
        return result;
    }

    /// <summary>
    ///     获取在线且已登入的玩家昵称
    /// </summary>
    /// <returns>在线且已登入的玩家昵称的数组</returns>
    public async Task<string[]> GetActiveListAsync()
    {
        var result = await SendGetToEndpointAsync<Dictionary<string, string>>("v2/users/activelist",
            new Dictionary<string, object>
            {
                {"token", Token}
            });
        var activeUsers = result["activeusers"];
        if (activeUsers.Length == 0) return Array.Empty<string>();
        return activeUsers.Split("\t");
    }

    /// <summary>
    ///     获取服务器状态信息
    /// </summary>
    /// <returns>服务器状态对象</returns>
    public async Task<ServerStatus> GetServerStatusAsync()
    {
        return await SendGetToEndpointAsync<ServerStatus>("v2/server/status", new Dictionary<string, object>
        {
            {"token", Token},
            {"players", true}
        });
    }

    /// <summary>
    ///     执行远程指令
    /// </summary>
    /// <param name="command">执行的指令（以/开头）</param>
    /// <returns>API中response数组</returns>
    public async Task<string[]> ExecuteRemoteCommandAsync(string command)
    {
        var result = await SendGetToEndpointAsync<Dictionary<string, object>>("v3/server/rawcmd",
            new Dictionary<string, object>
            {
                {"token", Token},
                {"cmd", command}
            });
        return JsonSerializer.Deserialize<string[]>(result["response"].ToString());
    }

    public async Task<PlayerInfo> GetPlayerInfoAsync(Player player)
    {
        return await SendGetToEndpointAsync<PlayerInfo>("player/info", new Dictionary<string, object>
        {
            {"token", Token},
            {"player", player.UserName}
        });
    }
}