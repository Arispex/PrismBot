using System.Net;
using System.Runtime.Loader;
using GenHTTP.Engine;
using GenHTTP.Modules.Functional;
using GenHTTP.Modules.Functional.Provider;
using Microsoft.EntityFrameworkCore;
using PrismBot.SDK;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Static;
using Sora.Interfaces;
using YukariToolBox.LightLog;

namespace PrismBot;

public class PluginLoader
{
    public static List<Plugin> LoadedPlugins = new();
    public ISoraService Service;
    public InlineBuilder handle;

    public PluginLoader(ISoraService service)
    {
        Service = service;
        handle = Inline.Create();
    }

    public void LoadFromPluginPath(string path)
    {
        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        var types = assembly.GetTypes();

        foreach (var type in types)
            if (type.IsSubclassOf(typeof(Plugin)))
            {
                var o = Activator.CreateInstance(type);
                var p = o as Plugin;

                p.OnLoad();
                //获取配置
                var config = ConfigManager.GetBotConfig();
                //加载GenHttp
                var registeredEndPoints = p.GetRegisteredEndPoints();
                foreach (var registeredEndPoint in registeredEndPoints)
                {
                    handle = handle.Get(registeredEndPoint.Path, registeredEndPoint.Delegate);
                }
                //加载群聊事件
                var registeredGroupCommands = p.GetRegisteredGroupCommands();
                foreach (var registeredGroupCommand in registeredGroupCommands)
                    Service.Event.OnGroupMessage += async (eventType, args) =>
                    {
                        if (registeredGroupCommand.Match(eventType, args))
                        {
                            var botDbContext = new BotDbContext();
                            //获取玩家对象
                            var player = await botDbContext.Players
                                .Include(p => p.Group)
                                .ThenInclude(g => g.Parent)
                                .FirstOrDefaultAsync(p => p.QQ == args.Sender.Id);
                            //判断玩家是否是游客
                            if (await args.Sender.IsGuest())
                            {
                                //判断游客组是否有权限
                                var group = await botDbContext.Groups.FindAsync("Guest");
                                if (group == null)
                                {
                                    Log.Warning("System", "未找到Guest组");
                                    return;
                                }
                                //判断Guest组是否有权限
                                if (group.HasPermission(registeredGroupCommand.GetPermission()))
                                {
                                    await registeredGroupCommand.OnPermissionGranted(eventType, args);
                                    return;
                                }

                                await registeredGroupCommand.OnPermissionDenied(eventType, args);
                                return;
                            }
                            //判断玩家是否已经注册过了或者玩家是否没有权限
                            if ((player == null || !player.HasPermission(registeredGroupCommand.GetPermission())) && !args.IsSuperUser)
                            {
                                await registeredGroupCommand.OnPermissionDenied(eventType, args);
                                return;
                            }

                            await registeredGroupCommand.OnPermissionGranted(eventType, args);
                        }
                    };
                //加载私聊事件
                var registeredPrivateCommands = p.GetRegisteredPrivateCommands();
                foreach (var registeredPrivateCommand in registeredPrivateCommands)
                {
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
                            if (await args.Sender.IsGuest())
                            {
                                //判断游客组是否有权限
                                var group = await botDbContext.Groups.FindAsync("Guest");
                                if (group == null)
                                {
                                    Log.Warning("System", "未找到Guest组");
                                    return;
                                }
                                //判断Guest组是否有权限
                                if (group.HasPermission(registeredPrivateCommand.GetPermission()))
                                {
                                    await registeredPrivateCommand.OnPermissionGranted(eventType, args);
                                    return;
                                }

                                await registeredPrivateCommand.OnPermissionDenied(eventType, args);
                                return;
                            }

                            if ((player == null || !player.HasPermission(registeredPrivateCommand.GetPermission())) && !args.IsSuperUser)
                            {
                                await registeredPrivateCommand.OnPermissionDenied(eventType, args);
                                return;
                            }

                            await registeredPrivateCommand.OnPermissionGranted(eventType, args);
                        }
                    };
                }
                LoadedPlugins.Add(p);

                Log.Info("Plugin Loader",
                    $"{p.GetPluginName()} v{p.GetVersion()} (by {p.GetAuthor()}) initiated");
            }
    }

    public void StartGenHttp()
    {
        var config = ConfigManager.GetBotConfig();
        Log.Info("Plugin Loader", "正在启动 GenHttp...");
        var thread = new Thread(() =>
        {
            Host.Create()
                .Console()
                .Bind(IPAddress.Any, config.GenHttpPort)
                .Handler(handle)
                .Run();
        });
        Log.Info($"Plugin Loader", $"GenHttp 正在运行 [0.0.0.0:{config.GenHttpPort}]");
        thread.Start();
    }
}