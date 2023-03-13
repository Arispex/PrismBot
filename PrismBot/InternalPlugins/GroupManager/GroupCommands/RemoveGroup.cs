using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.GroupManager.GroupCommands;

public class RemoveGroup : IGroupCommand
{
    public string GetCommand()
    {
        return "删除组";
    }

    public string GetPermission()
    {
        return "gm.removegroup";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length != 2)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：删除组 <组名称>");
            return;
        }
        
        var db = new BotDbContext();
        var group = await db.Groups.FirstOrDefaultAsync(x => x.GroupName == args[1]);
        if (group == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("不存在该组。");
            return;
        }

        var player = await db.Players.FirstOrDefaultAsync(x => x.Group == group);
        if (player != null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("该组中仍有用户。请先将用户移出该组。");
            return;
        }

        db.Groups.Remove(group);
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage("删除成功。");
    }
}