using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.MessageLogger;

// ReSharper disable once UnusedType.Global
public class MessageLogger : Plugin
{
    public override string Name => "MessageLogger";

    public override string Version => "1.0.0";

    public override string Author => "Qianyiovo";

    public override string Description => "提供消息日志";

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(new GroupMessageLogger());
        CommandManager.RegisterPrivateCommand(new PrivateMessageLogger());
    }
}