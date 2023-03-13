using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Static;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.MenuHelper;

public class PrivateMenu : IPrivateCommand
{
    public string GetCommand()
    {
        return "菜单";
    }

    public string GetPermission()
    {
        return "menu.private";
    }

    public async Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        var Commands = CommandManager.RegisteredPrivateCommands.Select(x => x.GetCommand())
            .Where(x => x != string.Empty).ToList();
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length > 2)
        {
            await eventArgs.Sender.SendPrivateMessage("您输入的参数不符合要求。请参考以下语法进行输入：菜单 [页码]");
            return;
        }

        var page = args.Length == 2 ? int.Parse(args[1]) : 1;
        Commands = Commands.Skip((page - 1) * 20).Take(20).ToList();
        Commands.Remove("菜单");
        if (Commands.Count == 0 && page == 1)
        {
            await eventArgs.Sender.SendPrivateMessage("没有可用的私聊指令。");
            return;
        }

        if (Commands.Count == 0 && page != 1)
        {
            await eventArgs.Sender.SendPrivateMessage("没有更多的私聊指令了。");
            return;
        }

        await eventArgs.Sender.SendPrivateMessage(string.Join(", ", Commands));
    }
}