using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;
using YukariToolBox.LightLog;

namespace PrismBot.InternalPlugins.MessageLogger;

public class PrivateMessageLogger : IPrivateCommand
{
    public string GetCommand()
    {
        return "";
    }

    public string GetPermission()
    {
        return string.Empty;
    }

    public async Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
    }

    public async Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        Log.Info("MessageLogger",
            $"收到来自 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 的私聊消息：{eventArgs.Message.RawText}");
    }
}