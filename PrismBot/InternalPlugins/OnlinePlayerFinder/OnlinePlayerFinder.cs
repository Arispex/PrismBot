using PrismBot.InternalPlugins.OnlinePlayerFinder.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.OnlinePlayerFinder;

public class OnlinePlayerFinder : Plugin
{
    public override string GetPluginName()
    {
        return "OnlinePlayerFinder";
    }

    public override string GetVersion()
    {
        return "1.0.1";
    }

    public override string GetAuthor()
    {
        return "Qianyiovo";
    }

    public override string GetDescription()
    {
        return "获取在线玩家";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(this, new OnlinePlayer());
    }
}