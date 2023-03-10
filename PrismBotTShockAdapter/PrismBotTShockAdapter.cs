using Newtonsoft.Json;
using PrismBotTShockAdapter.Models;
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

    public override Version Version => new("1.0.2");

    public override void Initialize()
    {
        if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "tshock", "ElegantWhitelistTShockAdapter.json")))
        {
            File.WriteAllText(
                Path.Combine(Environment.CurrentDirectory, "tshock", "ElegantWhitelistTShockAdapter.json"),
                JsonConvert.SerializeObject(new ElegantWhitelistConfig()));
            TShock.Log.ConsoleWarn("未找到配置文件(ElegantWhitelistTShockAdapter.json)，已自动生成");
        }

        ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
    }

    private async void OnJoin(JoinEventArgs args)
    {
        var player = TShock.Players[args.Who];
        using var httpClient = new HttpClient();
        var config = JsonConvert.DeserializeObject<ElegantWhitelistConfig>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory,
            "tshock", "ElegantWhitelistTShockAdapter.json")));
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(
                $"http://{config.Host}:{config.Port}/elegantwhitelist/check?playerName={player.Name}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            player.Disconnect(config.ConnectionErrorMessage);
            return;
        }

        var result =
            JsonConvert.DeserializeObject<ElegantWhitelistCheckResponse>(await response.Content.ReadAsStringAsync());
        if (!result.Data.IsRegistered) player.Disconnect(config.NotInWhitelistMessage);
        if (result.Data.IsFreeze) player.Disconnect(config.AccountFrozenMessage);
    }
}