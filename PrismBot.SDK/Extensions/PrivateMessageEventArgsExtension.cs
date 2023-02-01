using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Extensions;

public static class PrivateMessageEventArgsExtension
{
    public static async Task SendDefaultPermissionDeniedMessage(this PrivateMessageEventArgs privateMessageEventArgs)
    {
        await privateMessageEventArgs.Sender.SendPrivateMessage("你无权访问此命令");
    }
}