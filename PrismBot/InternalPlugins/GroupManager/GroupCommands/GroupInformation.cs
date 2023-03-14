using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.GroupManager.GroupCommands;

public class GroupInformation : IGroupCommand
{
    public string GetCommand()
    {
        return "组信息";
    }

    public string GetPermission()
    {
        return "gm.groupinfo";
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
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：组信息 <组名称>");
            return;
        }
        var groups = new BotDbContext().Groups.Include(x => x.Parent).Where(x => x.GroupName == args[1]).ToList();
        if (!groups.Any())
        {
            await eventArgs.SourceGroup.SendGroupMessage("没有找到该组。");
            return;
        }
        foreach (var group in groups)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"组名称：{group.GroupName}\n组继承：{(group.Parent != null ? group.Parent.GroupName : "无")}\n组权限：{(group.GetPermissions().Count > 0 ? string.Join(", ", group.GetPermissions()) : "无")}");
        }
    }
}