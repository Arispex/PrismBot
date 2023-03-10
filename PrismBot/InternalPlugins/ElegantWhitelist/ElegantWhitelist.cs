using System.Text.Json;
using GenHTTP.Api.Protocol;
using PrismBot.InternalPlugins.ElegantWhitelist.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Data;
using PrismBot.SDK.Models;
using PrismBot.SDK.Static;
using YukariToolBox.LightLog;

namespace PrismBot.InternalPlugins.ElegantWhitelist;

public class ElegantWhitelist : Plugin
{
    public override string GetPluginName()
    {
        return "ElegantWhitelist";
    }

    public override string GetVersion()
    {
        return "1.0.3";
    }

    public override string GetAuthor()
    {
        return "Qianyiovo";
    }

    public override string GetDescription()
    {
        return "优雅的白名单";
    }

    public override void OnLoad()
    {
        if (!Directory.Exists(Path.Combine("plugins", "ElegantWhitelist")))
            Directory.CreateDirectory(Path.Combine("plugins", "ElegantWhitelist"));
        if (Config.CreateIfNotExist())
        {
            Log.Warning("ElegantWhitelist", "未找到配置文件，已自动生成。");
        }

        CommandManager.RegisterGroupCommand(this, new AddWhitelist());
        CommandManager.RegisterGroupCommand(this, new RemoveWhitelist());
        CommandManager.RegisterGroupCommand(this, new FreezeAccount());
        CommandManager.RegisterGroupCommand(this, new UnfreezeAccount());
        CommandManager.RegisterGroupCommand(this, new MyInformation());
        CommandManager.RegisterGroupCommand(this, new UserInformation());
        CommandManager.RegisterGroupCommand(this, new AddUserPermission());
        CommandManager.RegisterGroupCommand(this, new RemoveUserPermission());
        EndPointManager.RegisterEndPoint(new EndPoint("elegantwhitelist/check", (IRequest req) =>
        {
            if (!req.Query.TryGetValue("playerName", out var playerName))
                return JsonSerializer.Serialize(new Dictionary<string, object>
                {
                    {"status", "success"},
                    {"message", "缺少必要参数 playerName"}
                });

            var db = new BotDbContext();
            var player = db.Players.FirstOrDefault(x => x.UserName == playerName);

            var data = new Dictionary<string, string>
            {
                {"playerName", playerName},
                {"isRegistered", (player != null).ToString()},
                {"isFreeze", (player?.IsFreeze ?? false).ToString()}
            };

            return JsonSerializer.Serialize(new Dictionary<string, object>
            {
                {"status", "success"},
                {"message", player == null ? "玩家不存在" : player.IsFreeze ? "玩家已被冻结" : "玩家已注册"},
                {"data", data}
            });
        }));
    }
}