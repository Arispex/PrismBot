using PrismBot.SDK.Data;
using PrismBot.SDK.Exceptions;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ServerStatus.GroupCommands;

public class OnlinePlayer : IGroupCommand
{
    public string GetCommand()
    {
        return "在线";
    }

    public string GetPermission()
    {
        return "ss.onlineplayer";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var db = new BotDbContext();
        var servers = db.Servers.ToArray();
        var messages = new List<string>();
        if (servers.Length == 0)
        {
            await eventArgs.SourceGroup.SendGroupMessage("群主很懒，没有添加任何服务器。");
            return;
        }
        foreach (var server in servers)
        {
            SDK.Models.ServerStatus serverStatus;
            try
            {
                serverStatus = await server.GetServerStatusAsync();
            }
            catch (HttpRequestException)
            {
                messages.Add($"无法连接至 {server.ServerName}，请确认服务器已启动。");
                continue;
            }
            catch (InvalidToken)
            {
                messages.Add($"无法连接至 {server.ServerName}，请检查您的 token 是否正确并且未过期。");
                continue;
            }

            var message = $"以下玩家正在游玩 {server.ServerName} ({serverStatus.PlayerCount}/{serverStatus.MaxPlayers})：\n";
            foreach (var player in serverStatus.Players)
                if (player.Active)
                {
                    if (player.Group == "guest")
                        message += $"[{player.Nickname}(未登入)] ";
                    else
                        message += $"[{player.Nickname}] ";
                }

            message = message.TrimEnd();
            messages.Add(message);
        }

        await eventArgs.SourceGroup.SendGroupMessage(string.Join("\n\n", messages));
    }
}