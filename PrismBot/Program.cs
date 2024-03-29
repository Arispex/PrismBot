﻿using PrismBot;
using PrismBot.SDK.Data;
using PrismBot.SDK.Models;
using PrismBot.SDK.Static;
using Sora;
using Sora.Entities.Segment.DataModel;
using Sora.Net.Config;
using Sora.Util;
using Spectre.Console;
using YukariToolBox.LightLog;
using Color = Spectre.Console.Color;

AnsiConsole.Write(new FigletText("PrismBot").Color(Color.Aqua));

//设置log等级
Log.LogConfiguration
    .EnableConsoleOutput()
    .SetLogLevel(LogLevel.Info);

Log.Info("System", "Github：https://github.com/Qianyiovo/PrismBot");
Log.Info("System", "Copyright © 2023-present Qianyiovo");
Log.Info("System", "本项目基于 AGPL v3.0 许可证授权发行，您可以在遵守许可证的前提下自由使用、复制、修改、发布和分发本项目");
Log.Info("System", "有关 AGPL v3.0 许可证的详细信息，请参阅 https://www.gnu.org/licenses/agpl-3.0.html");
Log.Info("System", "正在读取配置文件...");


//检测配置文件是否存在
if (Config.CreateIfNotExist())
{
    Log.Warning("System", "检测到配置文件不存在，已生成默认配置文件");
    Log.Warning("System", "请在config.yml中填写相关配置后重启");
    Log.Warning("System", "按任意键退出");
    Console.ReadKey();
    Environment.Exit(0);
}

//读取配置文件
var config = Config.Instance;
Log.Info("System", "配置文件读取成功");


//判断是否存在Guest组别
await using var db = new BotDbContext();
await db.Database.EnsureCreatedAsync();
var group = await db.Groups.FindAsync("Guest");
if (group == null)
{
    //不存在则自动创建
    await db.AddAsync(new Group("Guest", null));
    Log.Warning("System", "Guest组别不存在，已自动创建");
}

await db.SaveChangesAsync();


//实例化Sora服务
var service = SoraServiceFactory.CreateService(new ServerConfig
{
    Host = config.Host,
    Port = config.Port,
    AccessToken = config.AccessToken,
    UniversalPath = config.UniversalPath,
    SuperUsers = config.SuperUsers,
    BlockUsers = config.BlockUsers
});
//存放Sora服务
GlobalTracker.SoraService = service;

//images文件夹
var imageFolderPath = Path.Combine(Environment.CurrentDirectory, "images");
if (!Directory.Exists(imageFolderPath))
    Directory.CreateDirectory(imageFolderPath);
//imageCache文件夹
var imageCacheFolderPath = Path.Combine(Environment.CurrentDirectory, "imageCache");
if (!Directory.Exists(imageCacheFolderPath))
    Directory.CreateDirectory(imageCacheFolderPath);
//检测插件目录是否存在
var pluginFolderPath = Path.Combine(Environment.CurrentDirectory, "plugins");
if (!Directory.Exists(pluginFolderPath))
    //不存在则创建
    Directory.CreateDirectory(pluginFolderPath);
// 加载所有插件
PluginLoader.LoadPlugins();


// 将所有 Handlers 附加到 Sora 上
CommandManager.Attach(service);
//启动GenHttp
EndPointManager.StartServer();

// 初始化消息日志
//收到群消息日志
service.Event.OnGroupMessage += (sender, eventArgs) =>
{
    Log.Info("System",
        $"收到来自群 {eventArgs.SourceGroup.GetGroupInfo().Result.groupInfo.GroupName}({eventArgs.SourceGroup.Id}) 内 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 的消息：{eventArgs.Message.RawText}");
    return ValueTask.CompletedTask;
};
//收到私聊消息日志
service.Event.OnPrivateMessage += (sender, eventArgs) =>
{
    Log.Info("System",
        $"收到来自 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 的私聊消息：{eventArgs.Message.RawText}");
    return ValueTask.CompletedTask;
};
//发送群聊消息日志
service.Event.OnSelfGroupMessage += (sender, eventArgs) =>
{
    Log.Info("System",
        $"向群 {eventArgs.SourceGroup.GetGroupInfo().Result.groupInfo.GroupName}({eventArgs.SourceGroup.Id}) 发送了消息：{eventArgs.Message.RawText}");
    return ValueTask.CompletedTask;
};
//发送私聊消息日志
service.Event.OnSelfPrivateMessage += (sender, eventArgs) =>
{
    Log.Info("System",
        $"向 {eventArgs.SenderInfo.Nick}({eventArgs.SenderInfo.UserId}) 发送了私聊消息：{eventArgs.Message.RawText}");
    return ValueTask.CompletedTask;
};

//启动服务并捕捉错误
await service.StartService()
    .RunCatch(e => Log.Error("Sora Service", Log.ErrorLogBuilder(e)));
await Task.Delay(-1);