using PrismBot;
using PrismBot.plugins.MessageLogger;
using PrismBot.SDK.Data;
using PrismBot.SDK.Models;
using PrismBot.SDK.Singletons;
using PrismBot.SDK.Static;
using Sora;
using Sora.Net.Config;
using Sora.Util;
using Spectre.Console;
using YamlDotNet.Serialization;
using YukariToolBox.LightLog;

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
try
{
    ConfigManager.GetBotConfig();
}
catch (FileNotFoundException)
{
    var serializer = new Serializer();
    var configPath = Path.Combine(Environment.CurrentDirectory, "config.yml");
    await using (var configStreamWriter = new StreamWriter(configPath))
    {
        var defaultConfig = new Config();
        serializer.Serialize(configStreamWriter, defaultConfig);
    }

    Log.Warning("System", "检测到配置文件不存在，已生成默认配置文件");
    Log.Warning("System", "请在config.yml中填写相关配置后重启");
    Log.Warning("System", "按任意键退出");
    Console.ReadKey();
    Environment.Exit(0);
}

//读取配置文件
var config = ConfigManager.GetBotConfig();
Log.Info("System", "配置文件读取成功");
//检测插件目录是否存在
var pluginFolderPath = Path.Combine(Environment.CurrentDirectory, "plugins");
if (!Directory.Exists(pluginFolderPath))
    //不存在则创建
    Directory.CreateDirectory(pluginFolderPath);
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
SoraServiceSingleton.Instance.SoraService = service;
//获取plugins目录下的插件
var pluginFiles = Directory.GetFiles(pluginFolderPath);
//加载内置插件
PluginLoader.Load(new MessageLogger());
//加载插件
foreach (var pluginFile in pluginFiles)
    if (pluginFile.EndsWith(".dll"))
    {
        PluginLoader.LoadFromPath(pluginFile);
    }
//注册插件事件
PluginLoader.RegisterAll();
Log.Info("Plugin Loader", $"已加载 {PluginLoader.LoadedPlugins.Count} 个插件");
//判断是否存在Guest组别
var botDbContext = new BotDbContext();
var group = await botDbContext.Groups.FindAsync("Guest");
if (group == null)
{
    //不存在则自动创建
    await botDbContext.AddAsync(new Group("Guest", null));
    Log.Warning("System", "Guest组别不存在，已自动创建");
}

await botDbContext.SaveChangesAsync();
//启动GenHttp
PluginLoader.StartGenHttp();
//测试
//启动服务并捕捉错误
await service.StartService()
    .RunCatch(e => Log.Error("Sora Service", Log.ErrorLogBuilder(e)));
await Task.Delay(-1);