using System.Net;
using System.Runtime.Loader;
using GenHTTP.Engine;
using GenHTTP.Modules.Functional;
using GenHTTP.Modules.Functional.Provider;
using Microsoft.EntityFrameworkCore;
using PrismBot.SDK;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Singletons;
using PrismBot.SDK.Static;
using Sora.Interfaces;
using YukariToolBox.LightLog;
using EndPoint = PrismBot.SDK.Models.EndPoint;

namespace PrismBot;

public static class PluginLoader
{
    public static readonly List<Plugin> LoadedPlugins = new();
    private static readonly ISoraService? Service = SoraServiceSingleton.Instance.SoraService;
    private static readonly InlineBuilder Handle = Inline.Create();

    private static readonly HashSet<IGroupCommand> GroupCommands = new();

    private static readonly HashSet<IPrivateCommand> PrivateCommands = new();

    private static readonly HashSet<EndPoint> EndPoints = new();

    public static void Load(Plugin p)
    {
        p.OnLoad();
        //加载GenHttp
        var registeredEndPoints = p.GetRegisteredEndPoints();
        foreach (var registeredEndPoint in registeredEndPoints) EndPoints.Add(registeredEndPoint);

        //加载群聊事件
        var registeredGroupCommands = p.GetRegisteredGroupCommands();
        foreach (var registeredGroupCommand in registeredGroupCommands)
            GroupCommands.Add(registeredGroupCommand);
        //加载私聊事件
        var registeredPrivateCommands = p.GetRegisteredPrivateCommands();
        foreach (var registeredPrivateCommand in registeredPrivateCommands)
            PrivateCommands.Add(registeredPrivateCommand);

        LoadedPlugins.Add(p);

        Log.Info("Plugin Loader",
            $"{p.GetPluginName()} v{p.GetVersion()} (by {p.GetAuthor()}) initiated");
    }

    public static void LoadFromPath(string path)
    {
        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        var types = assembly.GetTypes();

        foreach (var type in types)
            if (type.IsSubclassOf(typeof(Plugin)))
            {
                var o = Activator.CreateInstance(type);
                var p = o as Plugin;
                Load(p);
            }
    }

    public static void RegisterAll()
    {
        var config = ConfigManager.GetBotConfig();
        foreach (var registeredGroupCommand in GroupCommands)
            Service.Event.OnGroupMessage += async (eventType, args) =>
            {
                //判断群是否在白名单内
                if (!config.Groups.Contains(args.SourceGroup.Id)) return;

                if (registeredGroupCommand.Match(eventType, args))
                {
                    var botDbContext = new BotDbContext();
                    //获取玩家对象
                    var player = await botDbContext.Players
                        .Include(p => p.Group)
                        .ThenInclude(g => g.Parent)
                        .FirstOrDefaultAsync(p => p.QQ == args.Sender.Id);
                    //判断玩家是否是游客
                    if (await args.Sender.IsGuestAsync())
                    {
                        //判断游客组是否存在
                        var group = await botDbContext.Groups.FindAsync("Guest");
                        if (group == null)
                        {
                            Log.Warning("System", "未找到Guest组");
                            return;
                        }

                        //判断Guest组是否有权限
                        if (group.HasPermission(registeredGroupCommand.GetPermission()) || args.IsSuperUser)
                        {
                            try
                            {
                                await registeredGroupCommand.OnPermissionGrantedAsync(eventType, args);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            return;
                        }

                        try
                        {
                            await registeredGroupCommand.OnPermissionDeniedAsync(eventType, args);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        return;
                    }

                    //判断玩家是否已经注册过了或者玩家是否没有权限
                    if ((player == null || !player.HasPermission(registeredGroupCommand.GetPermission())) &&
                        !args.IsSuperUser)
                    {
                        try
                        {
                            await registeredGroupCommand.OnPermissionDeniedAsync(eventType, args);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        return;
                    }

                    try
                    {
                        await registeredGroupCommand.OnPermissionGrantedAsync(eventType, args);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            };

        foreach (var registeredPrivateCommand in PrivateCommands)
            Service.Event.OnPrivateMessage += async (eventType, args) =>
            {
                if (registeredPrivateCommand.Match(eventType, args))
                {
                    var botDbContext = new BotDbContext();

                    var player = await botDbContext.Players
                        .Include(p => p.Group)
                        .ThenInclude(g => g.Parent)
                        .FirstOrDefaultAsync(p => p.QQ == args.Sender.Id);
                    //判断玩家是否是游客
                    if (await args.Sender.IsGuestAsync())
                    {
                        //判断游客组是否有权限
                        var group = await botDbContext.Groups.FindAsync("Guest");
                        if (group == null)
                        {
                            Log.Warning("System", "未找到Guest组");
                            return;
                        }

                        //判断Guest组是否有权限
                        if (group.HasPermission(registeredPrivateCommand.GetPermission()) || args.IsSuperUser)
                        {
                            try
                            {
                                await registeredPrivateCommand.OnPermissionGrantedAsync(eventType, args);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            return;
                        }

                        try
                        {
                            await registeredPrivateCommand.OnPermissionDeniedAsync(eventType, args);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        return;
                    }

                    if ((player == null || !player.HasPermission(registeredPrivateCommand.GetPermission())) &&
                        !args.IsSuperUser)
                    {
                        try
                        {
                            await registeredPrivateCommand.OnPermissionDeniedAsync(eventType, args);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        return;
                    }

                    try
                    {
                        await registeredPrivateCommand.OnPermissionGrantedAsync(eventType, args);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            };

        foreach (var endPoint in EndPoints) Handle.Get(endPoint.Path, endPoint.Delegate);
    }

    public static void StartGenHttp()
    {
        var config = ConfigManager.GetBotConfig();
        Log.Info("Plugin Loader", "正在启动 GenHttp...");
        var thread = new Thread(() =>
        {
            Host.Create()
                .Console()
                .Bind(IPAddress.Any, config.GenHttpPort)
                .Handler(Handle)
                .Run();
        });
        Log.Info("Plugin Loader", $"GenHttp 正在运行 [0.0.0.0:{config.GenHttpPort}]");
        thread.Start();
    }
}