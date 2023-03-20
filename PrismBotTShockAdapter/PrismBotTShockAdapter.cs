using Newtonsoft.Json;
using PrismBotTShockAdapter.Models;
using Rests;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

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

        ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        TShock.RestApi.Register("/player/info", OnPlayerInventory);
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

        return new RestObject()
        {
            {"response", TShock.CharacterDB.GetPlayerData(null, player.ID)}
        };
    }
}