using PrismBot.InternalPlugins.PlugMan.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.PlugMan;

public class PlugMan : Plugin
{
    public override string GetPluginName()
    {
        return "PlugMan";
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
        return "插件管理器";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(this, new PluginList());
        CommandManager.RegisterGroupCommand(this, new PluginInfo());
    }
}