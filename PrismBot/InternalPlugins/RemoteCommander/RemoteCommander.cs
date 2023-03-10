using PrismBot.InternalPlugins.RemoteCommander.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.RemoteCommander;

public class RemoteCommander : Plugin
{
    public override string GetPluginName()
    {
        return "RemoteCommander";
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
        return "远程指令插件";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(this, new ExecuteCommand());
    }
}