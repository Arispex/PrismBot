using MySql.Data.MySqlClient;
using TShockAPI.DB;
using TShockAPI;
using Rests;
using TShockAPI.Hooks;

namespace PrismBotTShockAdapter.Modules;

// ReSharper disable once UnusedType.Global
public class Ranking : ModuleBase
{
    public override void Initialize()
    {
        // Database Initialization

        var tableCreator = new SqlTableCreator(
            TShock.DB,
            TShock.DB.GetSqlType() == SqlType.Sqlite ? new SqliteQueryCreator() : new MysqlQueryCreator());

        var deathRankingTable = new SqlTable("PB_DeathRanking",
            new SqlColumn("AccountID", MySqlDbType.Int32) { Primary = true },
            new SqlColumn("DeathCount", MySqlDbType.Int32));
        tableCreator.EnsureTableStructure(deathRankingTable);

        var onlineTimeTable = new SqlTable("PB_OnlineTimeRanking",
            new SqlColumn("AccountID", MySqlDbType.Int32) { Primary = true },
            new SqlColumn("OnlineTicks", MySqlDbType.Int64));
        tableCreator.EnsureTableStructure(onlineTimeTable);


        // Registration and Hooking

        TShockAPI.GetDataHandlers.KillMe.Register(OnPlayerDeath);
        TShock.RestApi.Register(new SecureRestCommand(
            "/prismbot/ranking/death",
            OnRestDeathRanking,
            "prismbot.ranking.death"));

        TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += OnPlayerLogin;
        TShockAPI.Hooks.PlayerHooks.PlayerLogout += OnPlayerLogout;
        TShock.RestApi.Register(new SecureRestCommand(
            "/prismbot/ranking/online_time",
            OnRestOnlineTimeRanking,
            "prismbot.ranking.online_time"));
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
            TShock.Log.ConsoleWarn($"[PrismBotAdapter/Ranking] Exception occur at {nameof(OnPlayerDeath)}, Ex:\n{ex}");
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

    #region Online Time Ranking

    private readonly Dictionary<int, DateTime> PlayerJoinTimes = new();

    private void OnPlayerLogin(PlayerPostLoginEventArgs e)
    {
        try
        {
            PlayerJoinTimes[e.Player.Account.ID] = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleWarn($"[PrismBotAdapter/Ranking] Exception occur at {nameof(OnPlayerLogin)}, Ex:\n{ex}");
        }
    }

    private void OnPlayerLogout(PlayerLogoutEventArgs e)
    {
        try
        {
            if (!PlayerJoinTimes.Remove(e.Player.Account.ID, out var joinTime))
                return;
            var sessionTimespan = DateTime.UtcNow - joinTime;

            TShock.DB.Query(
                Utils.SwitchDBQuery(
                    "INSERT INTO PB_OnlineTimeRanking (AccountID, OnlineTicks) VALUES (@0, @1) ON DUPLICATE KEY UPDATE OnlineTicks=OnlineTicks+@1",
                    "INSERT INTO PB_OnlineTimeRanking (AccountID, OnlineTicks) VALUES (@0, @1) ON CONFLICT(AccountID) DO UPDATE SET OnlineTicks=OnlineTicks+@1"),
                e.Player.Account.ID, sessionTimespan.Ticks);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleWarn($"[PrismBotAdapter/Ranking] Exception occur at {nameof(OnPlayerLogout)}, Ex:\n{ex}");
        }
    }

    private object OnRestOnlineTimeRanking(RestRequestArgs args)
    {
        var ranking = new List<dynamic>();
        using (var reader =
               TShock.DB.QueryReader(
                   "SELECT Users.Username AS Username, OnlineTicks FROM PB_OnlineTimeRanking INNER JOIN Users ON PB_OnlineTimeRanking.AccountID=Users.ID ORDER BY OnlineTicks DESC"))
        {
            while (reader.Read())
            {
                ranking.Add(new
                {
                    PlayerName = reader.Get<string>("Username"),
                    OnlineTicks = reader.Get<long>("OnlineTicks").ToString()
                });
            }
        }

        return new RestObject
        {
            { "ranking", ranking }
        };
    }

    #endregion
}