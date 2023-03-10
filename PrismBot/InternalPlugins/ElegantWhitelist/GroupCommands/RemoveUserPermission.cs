using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ElegantWhitelist.GroupCommands;

public class RemoveUserPermission : IGroupCommand
{
    public string GetCommand()
    {
        return "删除用户权限";
    }

    public string GetPermission()
    {
        return "ewl.removeuserpermission";
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
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：删除用户权限 <角色昵称/QQ/at> <权限名>");
            return;
        }
        
        //获取玩家
        var db = new BotDbContext();
        Player? player = null;
        //判断有无at
        if (eventArgs.Message.GetAllAtList().Count() == 1)
            player = await db.Players.Include(x => x.Group).ThenInclude(x => x.Parent).FirstOrDefaultAsync(x => x.QQ == eventArgs.Message.GetAllAtList().First());

        if (player == null)
        {
            //判断是否为QQ号
            if (long.TryParse(args[1], out var qq))
            {
                player = await db.Players.Include(x => x.Group).ThenInclude(x => x.Parent).FirstOrDefaultAsync(x => x.QQ == qq) ?? await db.Players.Include(x => x.Group).ThenInclude(x => x.Parent).FirstOrDefaultAsync(x => x.UserName == args[1]);
            }
            else
                //判断是否为角色昵称
                player = await db.Players.Include(x => x.Group).ThenInclude(x => x.Parent).FirstOrDefaultAsync(x => x.UserName == args[1]);
        }

        if (player == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("未找到该用户。");
            return;
        }
        
        //判断是否没有该权限
        if (!player.HasPermission(args[2]))
        {
            await eventArgs.SourceGroup.SendGroupMessage("该用户没有此权限");
            return;
        }
        
        player.RemovePermission(args[2]);
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage("已成功删除权限。");
    }
}