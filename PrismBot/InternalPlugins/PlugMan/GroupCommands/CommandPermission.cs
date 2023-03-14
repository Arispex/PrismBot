using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Static;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.PlugMan.GroupCommands;

public class CommandPermission : IGroupCommand
{
    public string GetCommand()
    {
        return "命令权限";
    }

    public string GetPermission()
    {
        return "pm.commandpermission";
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
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：命令权限 <命令名称>");
            return;
        }

        var command = CommandManager.RegisteredGroupCommands.FirstOrDefault(x => x.GetCommand() == args[1]);
        if (command == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("不存在该命令。");
            return;
        }
        await eventArgs.SourceGroup.SendGroupMessage($"{command.GetPermission()}");
    }
}