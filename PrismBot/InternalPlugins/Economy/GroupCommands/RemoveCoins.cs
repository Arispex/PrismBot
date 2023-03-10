using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.Economy.GroupCommands;

public class RemoveCoins : IGroupCommand
{
    public string GetCommand()
    {
        return "扣除硬币";
    }

    public string GetPermission()
    {
        return "econ.removecoins";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        //判断语法是否正确
        if (args.Length != 3)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：扣除硬币 <角色昵称/QQ/at> <数量>");
            return;
        }
        //获取玩家
        var db = new BotDbContext();
        Player? player = null;
        //判断有无at
        if (eventArgs.Message.GetAllAtList().Count() == 1)
            player = await db.Players.Include(x => x.Group).FirstOrDefaultAsync(x => x.QQ == eventArgs.Message.GetAllAtList().First());

        if (player == null)
        {
            //判断是否为QQ号
            if (long.TryParse(args[1], out var qq))
            {
                player = await db.Players.Include(x => x.Group).FirstOrDefaultAsync(x => x.QQ == qq) ?? await db.Players.Include(x => x.Group).FirstOrDefaultAsync(x => x.UserName == args[1]);
            }
            else
                //判断是否为角色昵称
                player = await db.Players.Include(x => x.Group).FirstOrDefaultAsync(x => x.UserName == args[1]);
        }

        if (player == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("未找到该用户。");
            return;
        }
        //判断数量是否为数字
        if (!int.TryParse(args[2], out var amount))
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。数量必须为一个数字。");
            return;
        }
        //判断数量是否为负数
        if (amount < 0)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。数量必须为一个正数。");
            return;
        }
        
        if (player.Coins < amount)
        {
            await eventArgs.SourceGroup.SendGroupMessage("该用户的硬币不足。");
            return;
        }

        player.Coins -= amount;
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage("扣除成功！");
    }
}