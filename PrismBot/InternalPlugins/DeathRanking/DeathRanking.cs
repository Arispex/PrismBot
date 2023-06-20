using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PrismBot.SDK;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Static;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.DeathRanking;

public class DeathRanking : Plugin
{
    public override string GetPluginName() => "Death Ranking";

    public override string GetVersion() => "1.0.0";

    public override string GetAuthor() => "LaoSparrow";

    public override string GetDescription() => "死亡排行榜";

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(this, new RankingGroupCommand());
    }
}

public class RankingGroupCommand : IGroupCommand
{
    private const int PAGE_SIZE = 10;

    public string GetCommand() => "死亡排行榜";

    public string GetPermission() => "deathranking.list";

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length < 2)
        {
            await eventArgs.SourceGroup.SendGroupMessage("用法：死亡排行榜 <服务器标识符> [页数]");
            return;
        }

        var currentPage = 1;
        if (args.Length >= 3 && !int.TryParse(args[2], out currentPage))
        {
            await eventArgs.SourceGroup.SendGroupMessage("页数需为数字并大于0");
            return;
        }
        if (currentPage <= 0)
        {
            await eventArgs.SourceGroup.SendGroupMessage("页数需为数字并大于0");
            return;
        }

        await using var db = new BotDbContext();
        var server = await db.Servers.FirstOrDefaultAsync(x => x.Identity == args[1]);
        if (server == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"未找到标识符为 {args[1]} 的服务器。");
            return;
        }

        var result = await server.SendGetToEndpointAsync<DeathRankingRespond>("prismbot/death_ranking", new Dictionary<string, object>
        {
            { "token", server.Token }
        });
        if (result.Ranking == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"服务器发生内部错误 ({nameof(result.Ranking)}值为空)");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendFormat("服务器: {0}({1})\n", server.ServerName, server.Identity);
        sb.Append("---死亡排行榜---\n");
        foreach (var r in result.Ranking.Skip((currentPage - 1) * PAGE_SIZE).Take(PAGE_SIZE))
        {
            sb.AppendFormat("{0}: {1}\n", r.PlayerName, r.DeathCount);
        }

        sb.AppendFormat("---页: <{0}/{1}>---", currentPage, result.Ranking.Length / PAGE_SIZE + 1);
        await eventArgs.SourceGroup.SendGroupMessage(sb.ToString());
    }

    public class DeathRankingRespond
    {
        public class DeathRankingRecord
        {
            public string PlayerName { get; set; }
            public int DeathCount { get; set; }
        }

        [JsonPropertyName("ranking")] public DeathRankingRecord[]? Ranking { get; set; }
    }
}