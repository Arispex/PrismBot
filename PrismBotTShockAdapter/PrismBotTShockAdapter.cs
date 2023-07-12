using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PrismBotTShockAdapter.Models;
using Rests;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace PrismBotTShockAdapter;

[ApiVersion(2, 1)]
public class PrismBotTShockAdapter : TerrariaPlugin
{
    public PrismBotTShockAdapter(Main game) : base(game)
    {
    }

    public override string Author => "Qianyiovo";

    public override string Description => "PrismBot TShock Adapter";

    public override string Name => "PrismBotTShockAdapter";

    public override Version Version => new("1.0.4");

    public override void Initialize()
    {
        var prismBotDirectory = Path.Combine(AppContext.BaseDirectory, "tshock", "PrismBot");
        if (!Directory.Exists(prismBotDirectory))
        {
            Directory.CreateDirectory(prismBotDirectory);
        }

        var configPath = Path.Combine(AppContext.BaseDirectory, "tshock", "PrismBot", "config.json");
        if (!File.Exists(configPath))
        {
            File.WriteAllText(
                configPath,
                JsonConvert.SerializeObject(new Config()));
            TShock.Log.ConsoleWarn("未找到配置文件(tshock/PrismBot/config.json)，已自动生成");
        }

        var elegantWhitelistPath = Path.Combine(AppContext.BaseDirectory, "tshock", "PrismBot", "elegantWhitelist.json");
        if (!File.Exists(elegantWhitelistPath))
        {
            File.WriteAllText(
                elegantWhitelistPath,
                JsonConvert.SerializeObject(new ElegantWhitelist()));
            TShock.Log.ConsoleWarn("未找到配置文件(tshock/PrismBot/elegantWhitelist.json)，已自动生成");
        }


        // Database Initialization

        var tableCreator = new SqlTableCreator(
            TShock.DB,
            TShock.DB.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());

        var deathRankingTable = new SqlTable("PB_DeathRanking",
            new SqlColumn("AccountID", MySqlDbType.Int32) { Primary = true },
            new SqlColumn("DeathCount", MySqlDbType.Int32));
        tableCreator.EnsureTableStructure(deathRankingTable);


        // Registration and Hooking

        ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        TShock.RestApi.Register("/player/info", OnPlayerInventory);

        TShockAPI.GetDataHandlers.KillMe.Register(OnPlayerDeath);
        TShock.RestApi.Register("/prismbot/death_ranking", OnRestDeathRanking);

        TShock.RestApi.Register("/prismbot/progress", Progress);
    }

    private async void OnJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];
        using var httpClient = new HttpClient();
        var elegantWhitelistConfig = JsonConvert.DeserializeObject<ElegantWhitelist>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "tshock", "PrismBot", "elegantWhitelist.json")));
        var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "tshock", "PrismBot", "config.json")));
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(
                $"http://{config.Host}:{config.Port}/elegantwhitelist/check?playerName={player.Name}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            player.Disconnect(elegantWhitelistConfig.ConnectionErrorMessage);
            return;
        }

        var result =
            JsonConvert.DeserializeObject<ElegantWhitelistCheckResponse>(await response.Content.ReadAsStringAsync());
        if (!result.Data.IsRegistered) player.Disconnect(elegantWhitelistConfig.NotInWhitelistMessage);
        if (result.Data.IsFreeze) player.Disconnect(elegantWhitelistConfig.AccountFrozenMessage);
    }

    private object OnPlayerInventory(RestRequestArgs args)
    {
        var playerName = args.Parameters["player"];
        if (playerName == null)
        {
            var response = new RestObject()
            {
                {"error", "Missing player parameter"}
            };
            response["status"] = "400";
            return response;
        }

        var player = TShock.UserAccounts.GetUserAccountByName(playerName);
        if (player == null)
        {
            var response = new RestObject()
            {
                {"error", "Player not found"}
            };
            response["status"] = "400";
            return response;
        }

        var playerInfo = TShock.CharacterDB.GetPlayerData(null, player.ID);
        return new RestObject()
        {
            {"inventory", playerInfo.inventory}
        };
    }

    #region Death Ranking

    private void OnPlayerDeath(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        try
        {
            var player = TShock.Players[e.PlayerId];
            if (player.Account == null)
                return;

            TShock.DB.Query(
                Utils.SwitchDBQuery(
                    "INSERT INTO PB_DeathRanking (AccountID, DeathCount) VALUES (@0, 1) ON DUPLICATE KEY UPDATE DeathCount=DeathCount+1",
                    "INSERT INTO PB_DeathRanking (AccountID, DeathCount) VALUES (@0, 1) ON CONFLICT(AccountID) DO UPDATE SET DeathCount=DeathCount+1"),
                player.Account.ID);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleWarn($"[PrismBotAdapter] Exception occur at {nameof(OnPlayerDeath)}, Ex:\n{ex}");
        }
    }

    private object OnRestDeathRanking(RestRequestArgs args)
    {
        var ranking = new List<dynamic>();
        using (var reader =
               TShock.DB.QueryReader(
                   "SELECT Users.Username AS Username, DeathCount FROM PB_DeathRanking INNER JOIN Users ON PB_DeathRanking.AccountID=Users.ID ORDER BY DeathCount DESC"))
        {
            while (reader.Read())
            {
                ranking.Add(new
                {
                    PlayerName = reader.Get<string>("Username"),
                    DeathCount = reader.Get<int>("DeathCount")
                });
            }
        }

        return new RestObject
        {
            { "ranking", ranking }
        };
    }

    #endregion
    private object Progress(RestRequestArgs args)//获取进度详情
        {
            Dictionary<string, bool> progress = new Dictionary<string, bool>()
            {
                {"King Slime", NPC.downedSlimeKing}, //史莱姆王
                {"Eye of Cthulhu", NPC.downedBoss1}, //克苏鲁之眼
                {"Eater of Worlds / Brain of Cthulhu", NPC.downedBoss2}, //世界吞噬者 或 克苏鲁之脑
                {"Queen Bee", NPC.downedQueenBee}, //蜂后
                {"Skeletron", NPC.downedBoss3}, //骷髅王
                {"Deerclops", NPC.downedDeerclops}, //巨鹿
                {"Wall of Flesh", Main.hardMode}, //肉山
                {"Queen Slime", NPC.downedQueenSlime}, //史莱姆皇后
                {"The Twins", NPC.downedMechBoss2}, //双子魔眼
                {"The Destroyer", NPC.downedMechBoss1}, //毁灭者
                {"Skeletron Prime", NPC.downedMechBoss3}, //机械骷髅王
                {"Plantera", NPC.downedPlantBoss}, //世纪之花
                {"Golem", NPC.downedGolemBoss}, //石巨人
                {"Duke Fishron", NPC.downedFishron}, // 朱鲨
                {"Empress of Light", NPC.downedEmpressOfLight}, //光女
                {"Lunatic Cultist", NPC.downedAncientCultist}, //教徒
                {"Moon Lord", NPC.downedMoonlord}, //月总
                {"Solar Pillar", NPC.downedTowerSolar}, //太阳能柱
                {"Nebula Pillar", NPC.downedTowerNebula}, //星云柱
                {"Vortex Pillar", NPC.downedTowerVortex}, //涡柱
                {"Stardust Pillar", NPC.downedTowerStardust}, //星尘柱
            };
            return new RestObject()
            {
                {
                    "response",
                     progress
                }
            };
        }
}