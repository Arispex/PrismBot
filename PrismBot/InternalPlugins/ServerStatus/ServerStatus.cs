using PrismBot.InternalPlugins.ServerStatus.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;
using YukariToolBox.LightLog;

namespace PrismBot.InternalPlugins.ServerStatus;

public class ServerStatus : Plugin
{
    public override string GetPluginName()
    {
        return "OnlinePlayerFinder";
    }

    public override string GetVersion()
    {
        return "1.0.2";
    }

    public override string GetAuthor()
    {
        return "Qianyiovo";
    }

    public override string GetDescription()
    {
        return "获取服务器信息";
    }

    public override void OnLoad()
    {
        //检测图片资源是否存在
        if (!File.Exists(Path.Combine("images", "frame.png")) || !File.Exists(Path.Combine("images", "background.png"))
            || !Directory.Exists(Path.Combine("images", "items")))
        {
            Log.Error("ServerStatus", "检测到图片资源丢失，请在官网重新下载。");
            Log.Error("ServerStatus", "按任意键退出");
            Console.ReadKey();
            Environment.Exit(0);
        }
        CommandManager.RegisterGroupCommand(this, new OnlinePlayer());
        CommandManager.RegisterGroupCommand(this, new MyBag());
        CommandManager.RegisterGroupCommand(this, new UserBag());
    }
}