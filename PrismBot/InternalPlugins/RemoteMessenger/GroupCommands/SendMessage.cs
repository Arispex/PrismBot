using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Exceptions;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.RemoteMessenger.GroupCommands;

public class SendMessage : IGroupCommand
{
    public string GetCommand()
    {
        return "发送消息";
    }

    public string GetPermission()
    {
        return "rm.sendmessage";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length < 3)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：发送消息 <服务器标识符> <消息>");
            return;
        }

        var db = new BotDbContext();
        var server = await db.Servers.FirstOrDefaultAsync(s => s.Identity == args[1]);
        if (server == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"不存在标识符为 {args[1]} 的服务器。");
            return;
        }

        var player = await db.Players.FindAsync(eventArgs.Sender.Id);
        string message;
        if (player == null)
            message = $"{eventArgs.SenderInfo.Nick}({eventArgs.Sender.Id}): {string.Join(" ", args.Skip(2).ToArray())}";
        else
            message = $"{player.UserName}({eventArgs.Sender.Id}): {string.Join(" ", args.Skip(2).ToArray())}";
        try
        {
            await server.ExecuteRemoteCommandAsync($"/say {message}");

            await eventArgs.SourceGroup.SendGroupMessage("发送成功！");
        }
        catch (HttpRequestException)
        {
            await eventArgs.SourceGroup.SendGroupMessage("无法连接至服务器，请确认服务器已启动。");
        }
        catch (InvalidToken)
        {
            await eventArgs.SourceGroup.SendGroupMessage("无法连接至服务器，请检查您的 token 是否正确并且未过期。");
        }
    }
}