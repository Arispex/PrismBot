using PrismBot.InternalPlugins.ServerStatus.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.ServerStatus;

public class OnlinePlayerFinder : Plugin
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
        CommandManager.RegisterGroupCommand(this, new OnlinePlayer());
    }
}