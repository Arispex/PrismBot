using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.GroupManager.GroupCommands;

public class ModifyGroupParent : IGroupCommand
{
    public string GetCommand()
    {
        return "修改组继承";
    }

    public string GetPermission()
    {
        return "gm.modifygroupparent";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length != 3)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：修改组继承 <组名称> <继承组名称/无>");
            return;
        }

        var db = new BotDbContext();
        var group = await db.Groups.Include(x => x.Parent).FirstOrDefaultAsync(x => x.GroupName == args[1]);
        if (group == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("不存在该组。");
            return;
        }

        if (args[2] != "无")
        {
            var groupParent = await db.Groups.Include(x => x.Parent).FirstOrDefaultAsync(x => x.GroupName == args[2]);
            if (groupParent == null)
            {
                await eventArgs.SourceGroup.SendGroupMessage("不存在该继承组。");
                return;
            }

            if (group.Parent == groupParent)
            {
                await eventArgs.SourceGroup.SendGroupMessage("该组已经继承该组。");
                return;
            }
            group.Parent = groupParent;
        }
        else
        {
            if (group.Parent == null)
            {
                await eventArgs.SourceGroup.SendGroupMessage("该组已经没有继承组。");
                return;
            }
            group.Parent = null;
        }
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage("修改成功。");
    }
}