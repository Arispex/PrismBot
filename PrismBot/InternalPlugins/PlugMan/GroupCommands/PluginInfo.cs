using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.PlugMan.GroupCommands;

public class PluginInfo : IGroupCommand
{
    public string GetCommand()
    {
        return "插件信息";
    }

    public string GetPermission()
    {
        return "pm.plugininfo";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length != 2)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：插件信息 <插件名称>");
            return;
        }
        var plugins = PluginLoader.Plugins.Where(x => x.GetPluginName() == args[1]);
        if (!plugins.Any())
        {
            await eventArgs.SourceGroup.SendGroupMessage("没有找到该插件。");
            return;
        }
        foreach (var plugin in plugins)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"插件名称：{plugin.GetPluginName()}\n插件作者：{plugin.GetAuthor()}\n插件版本：{plugin.GetVersion()}\n插件描述：{plugin.GetDescription()}");
        }
    }
}