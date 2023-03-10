using PrismBot.SDK.Utils;

namespace PrismBot.InternalPlugins.ElegantWhitelist;

public class Config : ConfigBase<Config>
{
    protected override string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, "plugins", "ElegantWhitelist", "config.yml");
    public string DefaultGroup = "Default";
    public bool AllowPureDigits = false;
    public bool AllowSpecialCharacters = false;
}