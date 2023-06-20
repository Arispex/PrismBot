using TShockAPI;

namespace PrismBotTShockAdapter;

public static class Utils
{
    public static string SwitchDBQuery(string mysqlQuery, string sqliteQuery) =>
        TShock.Config.Settings.StorageType.ToLower() == "mysql" ? mysqlQuery : sqliteQuery;
}