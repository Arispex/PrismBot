using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.GroupManager.GroupCommands;

public class RemoveGroupPermission : IGroupCommand
{
    public string GetCommand()
    {
        return "删除组权限";
    }

    public string GetPermission()
    {
        return "gm.removegrouppermission";
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
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：删除组权限 <组名称> <权限>");
            return;
        }

        var db = new BotDbContext();
        var group = await db.Groups.FindAsync(args[1]);
        if (group == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("不存在该组。");
            return;
        }

        if (!group.HasPermission(args[2]))
        {
            await eventArgs.SourceGroup.SendGroupMessage("该组没有该权限。");
            return;
        }
        
        group.RemovePermission(args[2]);
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage("删除成功。");
    }
}