using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.GroupManager.GroupCommands;

public class GroupList : IGroupCommand
{
    public string GetCommand()
    {
        return "组列表";
    }

    public string GetPermission()
    {
        return "gm.grouplist";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SourceGroup.SendGroupMessage(string.Join("\n", new BotDbContext().Groups.Select(x => x.GroupName).ToList()));
    }
}