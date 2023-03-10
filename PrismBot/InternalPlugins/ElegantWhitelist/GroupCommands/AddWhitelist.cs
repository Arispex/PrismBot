using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using Sora.EventArgs.SoraEvent;
using Config = PrismBot.InternalPlugins.ElegantWhitelist.Config;

namespace PrismBot.InternalPlugins.ElegantWhitelist.GroupCommands;

public class AddWhitelist : IGroupCommand
{
    public string GetCommand()
    {
        return "添加白名单";
    }

    public string GetPermission()
    {
        return "ewl.addwhitelist";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        //判断语法是否正确
        if (args.Length != 2)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：添加白名单 <角色昵称>");
            return;
        }

        //检查该用户是否已添加白名单
        var db = new BotDbContext();
        var user = await db.Players.FirstOrDefaultAsync(x => x.QQ == eventArgs.Sender.Id);
        if (user != null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您已经添加过白名单了。");
            return;
        }

        //检查改角色昵称是否已经被其他用户添加 
        var player = await db.Players.FirstOrDefaultAsync(x => x.UserName == args[1]);
        if (player != null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("该角色昵称已经被其他用户添加，请更换角色昵称。");
            return;
        }
        //获取配置文件
        var config = Config.Instance;
        //检查昵称是否为纯数字
        if (int.TryParse(args[1], out _) && config.AllowPureDigits == false)
        {
            await eventArgs.SourceGroup.SendGroupMessage("角色昵称不能为纯数字。");
            return;
        }

        //检查昵称是否含有特殊字符
        if (Regex.IsMatch(args[1], @"[^\u4e00-\u9fa5a-zA-Z0-9]") && config.AllowSpecialCharacters == false)
        {
            await eventArgs.SourceGroup.SendGroupMessage("角色昵称不能含有特殊字符。");
            return;
        }

        //添加白名单
        var defaultGroup = await db.Groups.FindAsync(config.DefaultGroup);
        if (defaultGroup == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"未找到默认组 {config.DefaultGroup}。");
            return;
        }

        await db.Players.AddAsync(new Player(eventArgs.Sender.Id, args[1], defaultGroup));
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage("添加白名单成功。");
    }
}