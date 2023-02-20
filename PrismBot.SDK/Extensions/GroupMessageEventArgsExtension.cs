using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Extensions;

public static class GroupMessageEventArgsExtension
{
    /// <summary>
    ///     默认权限拒绝事件，向群发送"你无权访问此命令"
    /// </summary>
    /// <param name="groupMessageEventArgs"></param>
    public static async Task SendDefaultPermissionDeniedMessageAsync(this GroupMessageEventArgs groupMessageEventArgs)
    {
        await groupMessageEventArgs.SourceGroup.SendGroupMessage("你无权访问此命令");
    }
}