using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ServerManager.PrivateCommands;

public class AddServer : IPrivateCommand
{
    public string GetCommand()
    {
        return "添加服务器";
    }

    public string GetPermission()
    {
        return "sm.addserver";
    }

    public async Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length != 6)
        {
            await eventArgs.Sender.SendPrivateMessage(
                "您输入的参数不符合要求。请参考以下语法进行输入：添加服务器 <服务器IP/域名> <服务器REST端口> <REST TOKEN> <对外显示昵称> <标识符>");
            return;
        }

        // 验证端口
        int port;
        var isPortValid = int.TryParse(args[2], out port);
        if (!isPortValid)
        {
            await eventArgs.Sender.SendPrivateMessage("您输入的参数不符合要求。端口必须为正整数。");
            return;
        }

        if (port < 1 || port > 65535)
        {
            await eventArgs.Sender.SendPrivateMessage("您输入的参数不符合要求。端口必须在 1～65535 之内。");
            return;
        }

        // 验证标识符是否已存在。
        var db = new BotDbContext();
        var server = await db.Servers.FindAsync(args[5]);
        if (server != null)
        {
            await eventArgs.Sender.SendPrivateMessage("该标识符已存在，请尝试使用其他标识符。");
            return;
        }

        // 添加服务器
        await db.Servers.AddAsync(
            new Server(args[4], args[1], port, args[3], args[5])
        );
        await db.SaveChangesAsync();
        await eventArgs.Sender.SendPrivateMessage("添加成功！");
    }
}