using Economy;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.Economy.GroupCommands;

public class SignIn : IGroupCommand
{

    public string GetCommand()
    {
        return "签到";
    }

    public string GetPermission()
    {
        return "econ.signin";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var db = new BotDbContext();
        var player = await db.Players.FindAsync(eventArgs.SenderInfo.UserId);
        if (player == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您还没有添加白名单");
            return;
        }

        if (player.IsFreeze)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您的账号已被冻结，请联系管理员");
            return;
        }

        if (player.IsSignedIn)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您已经签到过了，无法重复签到。");
            return;
        }
        
        player.IsSignedIn = true;
        var config = Config.Instance;
        var reward = new Random().Next(config.MinimumCoinReward, config.MaximumCoinReward + 1);
        player.Coins += reward;
        await db.SaveChangesAsync();
        await eventArgs.SourceGroup.SendGroupMessage($"恭喜您签到成功，获得了 {reward} 个硬币。");
    }
}