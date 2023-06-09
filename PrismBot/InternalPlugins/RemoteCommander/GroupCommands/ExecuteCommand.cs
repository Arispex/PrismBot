using System.Text;
using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Exceptions;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using SixLabors.ImageSharp.Drawing;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.RemoteCommander.GroupCommands;

public class ExecuteCommand : IGroupCommand
{
    public string GetCommand()
    {
        return "执行命令";
    }

    public string GetPermission()
    {
        return "rc.executecommand";
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
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：执行命令 <服务器标识符(all全服执行)> <命令(不需要/)>");
            return;
        }
        var command = "/" + string.Join(" ", args.Skip(2).ToArray());
        var db = new BotDbContext();
        if (args[1].ToLower() == "all" || args[1] == "*")
        {
            StringBuilder stringBuilder = new();
            foreach (var s in db.Servers)
            {
                try
                {
                    var result = await s.ExecuteRemoteCommandAsync(command);
                    if (result.Length == 0)
                    {
                        stringBuilder.AppendLine($"#️⃣服务器[{s.ServerName}]未返回了个寂寞。");
                        continue;
                    }
                    stringBuilder.AppendLine($"#️⃣服务器[{s.ServerName}]返回信息：\n{string.Join("\n", result)}");
                }
                catch (HttpRequestException)
                {
                    stringBuilder.AppendLine($"#️⃣无法连接至服务器，请确认服务器已启动。");
                }
                catch (InvalidTokenException)
                {
                    stringBuilder.AppendLine("#️⃣无法连接至服务器，请检查您的 token 是否正确并且未过期。");
                }
            }
            await eventArgs.SourceGroup.SendGroupMessage(stringBuilder.ToString());
            return;
        }
        var server = await db.Servers.FirstOrDefaultAsync(x => x.Identity == args[1]);
        if (server == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"未找到标识符为 {args[1]} 的服务器。");
            return;
        }

        try
        {
            var result = await server.ExecuteRemoteCommandAsync(command);
            if (result.Length == 0)
            {
                await eventArgs.SourceGroup.SendGroupMessage("未返回任何信息。");
                return;
            }

            await eventArgs.SourceGroup.SendGroupMessage(string.Join("\n", result));
        }
        catch (HttpRequestException)
        {
            await eventArgs.SourceGroup.SendGroupMessage("无法连接至服务器，请确认服务器已启动。");
        }
        catch (InvalidTokenException)
        {
            await eventArgs.SourceGroup.SendGroupMessage("无法连接至服务器，请检查您的 token 是否正确并且未过期。");
        }
    }
}