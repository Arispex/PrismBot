using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Static;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.MenuHelper;

public class GroupMenu : IGroupCommand
{
    public string GetCommand()
    {
        return "菜单";
    }

    public string GetPermission()
    {
        return "menu.group";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var Commands = CommandManager.RegisteredGroupCommands.Select(x => x.GetCommand())
            .Where(x => x != string.Empty).ToList();
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length > 2) await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：菜单 [页码]");

        var page = args.Length == 2 ? int.Parse(args[1]) : 1;
        Commands = Commands.Skip((page - 1) * 10).Take(10).ToList();
        Commands.Remove("菜单");
        if (Commands.Count == 0 && page == 1)
        {
            await eventArgs.SourceGroup.SendGroupMessage("没有可用的群聊指令。");
            return;
        }

        if (Commands.Count == 0 && page != 1)
        {
            await eventArgs.SourceGroup.SendGroupMessage("没有更多的群聊指令了。");
            return;
        }

        await eventArgs.SourceGroup.SendGroupMessage(string.Join(", ", Commands));
    }
}