using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.RemoteCommander.GroupCommands;

public class KickMe : IGroupCommand
{
    public string GetCommand()
    {
        return "自踢";
    }

    public string GetPermission()
    {
        return "rc.kickme";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var db = new BotDbContext();
        var player = await db.Players.FindAsync(eventArgs.SenderInfo.UserId);
        if (player == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您还没有添加白名单。");
            return;
        }
        foreach (var server in db.Servers)
        {
            await server.ExecuteRemoteCommandAsync($"/kick {player.UserName}");
        }

        await eventArgs.SourceGroup.SendGroupMessage("自踢成功。");
    }
}