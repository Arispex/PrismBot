using PrismBot.InternalPlugins.ServerManager.GroupCommands;
using PrismBot.InternalPlugins.ServerManager.PrivateCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.ServerManager;

public class ServerManager : Plugin
{
    public override string GetPluginName()
    {
        return "ServerManager";
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
        return "服务器管理插件";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterPrivateCommand(this, new AddServer());
        CommandManager.RegisterPrivateCommand(this, new RemoveServer());
        CommandManager.RegisterGroupCommand(this, new ServerList());
    }
}