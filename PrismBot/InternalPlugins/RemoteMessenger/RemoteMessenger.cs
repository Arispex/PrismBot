using PrismBot.InternalPlugins.RemoteMessenger.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.RemoteMessenger;

public class RemoteMessenger : Plugin
{
    public override string GetPluginName()
    {
        return "RemoteMessenger";
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
        return "远程发送服务器消息";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(this, new SendMessage());
    }
}