using System.Timers;
using Economy;
using PrismBot.InternalPlugins.Economy.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Data;
using PrismBot.SDK.Static;
using YukariToolBox.LightLog;
using Timer = System.Timers.Timer;

namespace PrismBot.InternalPlugins.Economy;

public class Economy : Plugin
{
    public override string GetPluginName()
    {
        return "Economy";
    }

    public override string GetVersion()
    {
        return "1.0.0";
    }

    public override string GetAuthor()
    {
        return "Qianyiovo";
    }

    public override string GetDescription()
    {
        return "经济插件";
    }

    public override void OnLoad()
    {
        // 创建配置文件
        if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "plugins", "Economy")))
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "plugins", "Economy"));
        }
        if (Config.CreateIfNotExist())
        {
            Log.Warning("Economy", "检测到配置文件不存在，已生成默认配置文件");
        }
        // 定时重置签到
        var timeUntilMidnight = DateTime.Today.AddDays(1) - DateTime.Now;
        var timer = new Timer(timeUntilMidnight.TotalMilliseconds);
        timer.Elapsed += ResetSignIn;
        var thread = new Thread(() => timer.Start());
        thread.Start();
        // 注册命令
        CommandManager.RegisterGroupCommand(this, new SignIn());
        CommandManager.RegisterGroupCommand(this, new AddCoins());
        CommandManager.RegisterGroupCommand(this, new RemoveCoins());
    }

    private async void ResetSignIn(object? sender, ElapsedEventArgs e)
    {
        var db = new BotDbContext();
        var players = db.Players;
        foreach (var player in players)
        {
            player.IsSignedIn = false;
        }
        await db.SaveChangesAsync();
        ((Timer) sender!).Interval = TimeSpan.FromDays(1).TotalMilliseconds;
        Log.Info("Economy", "重置签到成功");
    }
}