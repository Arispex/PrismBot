using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Extensions;

public static class PrivateMessageEventArgsExtension
{
    /// <summary>
    ///     默认权限拒绝事件，向发送者发送"你无权访问此命令"
    /// </summary>
    /// <param name="privateMessageEventArgs"></param>
    public static async Task SendDefaultPermissionDeniedMessageAsync(
        this PrivateMessageEventArgs privateMessageEventArgs)
    {
        await privateMessageEventArgs.Sender.SendPrivateMessage("你无权访问此命令");
    }
}