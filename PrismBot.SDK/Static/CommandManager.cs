using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using Sora.EventArgs.SoraEvent;
using Sora.Interfaces;
using YukariToolBox.LightLog;

namespace PrismBot.SDK.Static;

public static class CommandManager
{
    public static List<IGroupCommand> RegisteredGroupCommands = new();
    public static List<IPrivateCommand> RegisteredPrivateCommands = new();

    /// <summary>
    ///     注册群命令
    /// </summary>
    /// <param name="command">需要注册的对象</param>
    public static void RegisterGroupCommand(Plugin plugin, IGroupCommand command)
    {
        plugin.RegisteredGroupCommands.Add(command);
        RegisteredGroupCommands.Add(command);
    }

    /// <summary>
    ///     注册私聊命令
    /// </summary>
    /// <param name="command">需要注册的对象</param>
    public static void RegisterPrivateCommand(Plugin plugin,IPrivateCommand command)
    {
        plugin.RegisteredPrivateCommands.Add(command);
        RegisteredPrivateCommands.Add(command);
    }

    public static void Attach(ISoraService service)
    {
        service.Event.OnGroupMessage += OnGroupMessage;
        service.Event.OnPrivateMessage += OnPrivateMessage;
    }

    public static void Detach(ISoraService service)
    {
        service.Event.OnGroupMessage -= OnGroupMessage;
        service.Event.OnPrivateMessage -= OnPrivateMessage;
    }

    private static async ValueTask OnGroupMessage(string eventType, GroupMessageEventArgs args)
    {
        if (!Config.Instance.Groups.Contains(args.SourceGroup.Id))
            return;

        await using var db = new BotDbContext();
        var player = await db.Players
            .Include(p => p.Group)
            .ThenInclude(g => g.Parent)
            .FirstOrDefaultAsync(p => p.QQ == args.Sender.Id);
        var guest = await db.Groups.FindAsync("Guest");

        foreach (var command in RegisteredGroupCommands.Where(x => args.Message.GetCommandArgs()[0] == x.GetCommand()))
        {
            if (args.IsSuperUser)
            {
                await CatchAndLog(() =>
                    command.OnPermissionGrantedAsync(eventType, args));
                continue;
            }

            if (player?.HasPermission(command.GetPermission()) ?? false)
            {
                await CatchAndLog(() =>
                    command.OnPermissionGrantedAsync(eventType, args));
                continue;
            }

            if (player == null && (guest?.HasPermission(command.GetPermission()) ?? false))
            {
                await CatchAndLog(() =>
                    command.OnPermissionGrantedAsync(eventType, args));
                continue;
            }

            await CatchAndLog(() =>
                command.OnPermissionDeniedAsync(eventType, args));
        }
    }

    private static async ValueTask OnPrivateMessage(string eventType, PrivateMessageEventArgs args)
    {
        await using var db = new BotDbContext();
        var player = await db.Players
            .Include(p => p.Group)
            .ThenInclude(g => g.Parent)
            .FirstOrDefaultAsync(p => p.QQ == args.Sender.Id);
        var guest = await db.Groups.FindAsync("Guest");

        foreach (var command in RegisteredPrivateCommands.Where(x => args.Message.GetCommandArgs()[0] == x.GetCommand()))
        {
            if (args.IsSuperUser)
            {
                await CatchAndLog(() =>
                    command.OnPermissionGrantedAsync(eventType, args));
                continue;
            }

            if (player?.HasPermission(command.GetPermission()) ?? false)
            {
                await CatchAndLog(() =>
                    command.OnPermissionGrantedAsync(eventType, args));
                continue;
            }

            if (player == null && (guest?.HasPermission(command.GetPermission()) ?? false))
            {
                await CatchAndLog(() =>
                    command.OnPermissionGrantedAsync(eventType, args));
                continue;
            }

            await CatchAndLog(() =>
                command.OnPermissionDeniedAsync(eventType, args));
        }
    }

    private static async Task CatchAndLog(Func<Task> fn)
    {
        try
        {
            await fn();
        }
        catch (Exception ex)
        {
            Log.Warning(nameof(CommandManager), Log.ErrorLogBuilder(ex));
        }
    }
}