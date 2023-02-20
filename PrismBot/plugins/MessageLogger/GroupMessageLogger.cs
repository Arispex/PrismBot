using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;
using YukariToolBox.LightLog;

namespace PrismBot.plugins.MessageLogger;

public class GroupMessageLogger : IGroupCommand
{
    public bool Match(string type, BaseMessageEventArgs eventArgs)
    {
        return true;
    }

    public string GetPermission()
    {
        return string.Empty;
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        Log.Info("MessageLogger",
            $"收到来自群 {eventArgs.SourceGroup.GetGroupInfo().Result.groupInfo.GroupName}({eventArgs.SourceGroup.Id}) 内 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 的消息：{eventArgs.Message.RawText}");
    }
}