using System.Web;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ServerStatus.GroupCommands;

public class Wiki : IGroupCommand
{
    public string GetCommand()
    {
        return "wiki";
    }

    public string GetPermission()
    {
        return "ss.wiki";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        await eventArgs.SourceGroup.SendGroupMessage($"已为您在Wiki中搜索到以下内容：https://terraria.wiki.gg/zh/wiki/{HttpUtility.UrlEncode(args[1])}");
    }
}