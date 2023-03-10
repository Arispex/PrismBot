using PrismBot.SDK.Utils;
namespace Economy;

public class Config : ConfigBase<Config>
{
    protected override string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, "plugins", "Economy", "config.yml");
    public int MaximumCoinReward = 100;
    public int MinimumCoinReward = 10;
}