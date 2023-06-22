using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.Ranking;

public class Ranking : Plugin
{
    public override string GetPluginName() => "Ranking";

    public override string GetVersion() => "1.0.0";

    public override string GetAuthor() => "LaoSparrow";

    public override string GetDescription() => "排行榜";

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(this, new DeathRankingGroupCommand());
        CommandManager.RegisterGroupCommand(this, new OnlineTimeRankingGroupCommand());
    }
}

