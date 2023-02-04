using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Extensions;

public static class GroupMessageEventArgsExtension
{
    public static async Task SendDefaultPermissionDeniedMessageAsync(this GroupMessageEventArgs groupMessageEventArgs)
    {
        await groupMessageEventArgs.SourceGroup.SendGroupMessage("你无权访问此命令");
    }
}