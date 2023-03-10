using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ServerManager.PrivateCommands;

public class RemoveServer : IPrivateCommand
{
    public string GetCommand()
    {
        return "删除服务器";
    }

    public string GetPermission()
    {
        return "sm.removeserver";
    }

    public async Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length != 2)
        {
            await eventArgs.Sender.SendPrivateMessage("您输入的参数不符合要求。请参考以下语法进行输入：删除服务器 <标识符>");
            return;
        }

        var db = new BotDbContext();
        var server = await db.Servers.FindAsync(args[1]);
        if (server == null)
        {
            await eventArgs.Sender.SendPrivateMessage($"不存在标识符为 {args[1]} 的服务器。");
            return;
        }

        db.Servers.Remove(server);
        await db.SaveChangesAsync();
        await eventArgs.Sender.SendPrivateMessage("删除成功！");
    }
}