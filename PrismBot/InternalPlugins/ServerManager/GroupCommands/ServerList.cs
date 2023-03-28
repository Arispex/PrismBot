using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ServerManager.GroupCommands;

public class ServerList : IGroupCommand
{
    public string GetCommand()
    {
        return "服务器列表";
    }

    public string GetPermission()
    {
        return "sm.serverlist";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SourceGroup.SendGroupMessage(string.Join("\n",
            new BotDbContext().Servers.Select(x => $"{x.ServerName}({x.Identity})").ToList()));
    }
}