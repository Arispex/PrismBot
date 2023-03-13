using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.GroupManager.GroupCommands;

public class AddGroup : IGroupCommand
{
    public string GetCommand()
    {
        return "添加组";
    }

    public string GetPermission()
    {
        return "gm.addgroup";
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
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：添加组 <组名称>");
            return;
        }

        var db = new BotDbContext();
        var group = await db.Groups.FirstOrDefaultAsync(x => x.GroupName == args[1]);
        if (group != null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("该组已存在。");
            return;
        }

        await db.AddAsync(new Group(args[1], null));
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage("添加成功。");
    }
}