using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.plugins.MessageLogger;

public class MessageLogger : Plugin
{
    public override string GetPluginName()
    {
        return "MessageLogger";
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
        return "提供消息日志";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(new GroupMessageLogger());
        CommandManager.RegisterPrivateCommand(new PrivateMessageLogger());
    }
}