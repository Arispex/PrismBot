using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ElegantWhitelist.GroupCommands;

public class MyInformation : IGroupCommand
{
    public string GetCommand()
    {
        return "我的信息";
    }

    public string GetPermission()
    {
        return "ewl.myinfo";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var db = new BotDbContext();
        var player = await db.Players.Include(x => x.Group)
            .FirstOrDefaultAsync(x => x.QQ == eventArgs.SenderInfo.UserId);
        if (player == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您还没有添加白名单。");
            return;
        }

        await eventArgs.SourceGroup.SendGroupMessage($"QQ：{player.QQ}\n" +
                                                     $"角色昵称：{player.UserName}\n" +
                                                     $"组别：{player.Group.GroupName}\n" +
                                                     $"硬币：{player.Coins}\n" +
                                                     $"是否被冻结：{(player.IsFreeze ? '是':'否')}\n" +
                                                     $"权限：{(player.GetPermissions().Any() ? string.Join(",", player.GetPermissions()) : "无")}\n" +
                                                     $"注册时间：{player.RegistrationTime}");
    }
}