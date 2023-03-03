using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.PlugMan.GroupCommands;

public class PluginList : IGroupCommand
{
    public string GetCommand()
    {
        return "插件列表";
    }

    public string GetPermission()
    {
        return "pm.pluginlist";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var pluginNames = PluginLoader.Plugins.Select(x => x.GetPluginName()).ToList();
        await eventArgs.SourceGroup.SendGroupMessage(string.Join("\n", pluginNames));
    }
}