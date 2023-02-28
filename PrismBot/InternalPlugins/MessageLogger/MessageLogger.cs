using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.MessageLogger;

// ReSharper disable once UnusedType.Global
public class MessageLogger : Plugin
{
    public override string GetPluginName() => "MessageLogger";
    public override string GetVersion() => "1.0.0";
    public override string GetAuthor() => "Qianyiovo";
    public override string GetDescription() => "提供消息日志";

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(new GroupMessageLogger());
        CommandManager.RegisterPrivateCommand(new PrivateMessageLogger());
    }
}