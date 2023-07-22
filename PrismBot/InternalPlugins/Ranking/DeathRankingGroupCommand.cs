using PrismBot.SDK.Data;
using PrismBot.SDK.Interfaces;
using Sora.EventArgs.SoraEvent;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Extensions;

namespace PrismBot.InternalPlugins.Ranking;

public class DeathRankingGroupCommand : IGroupCommand
{
    private const int PAGE_SIZE = 10;

    public string GetCommand() => "死亡排行榜";

    public string GetPermission() => "ranking.death";

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
            currentPage = 1;
        currentPage = Math.Max(currentPage, 1);

        await using var db = new BotDbContext();
        var server = await db.Servers.FirstOrDefaultAsync(x => x.Identity == args[1]);
        if (server == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"未找到标识符为 {args[1]} 的服务器。");
            return;
        }

        var result = await server.SendGetToEndpointAsync<DeathRankingRespond>("prismbot/ranking/death", new Dictionary<string, object>
        {
            { "token", server.Token }
        });
        if (result.Ranking == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage($"服务器发生内部错误 ({nameof(result.Ranking)}值为空)");
            return;
        }

        var lastPageNum = result.Ranking.Length / PAGE_SIZE + Math.Min(result.Ranking.Length % PAGE_SIZE, 1);
        currentPage = Math.Min(currentPage, lastPageNum);

        var sb = new StringBuilder();
        sb.AppendFormat("服务器: {0}({1})\n", server.ServerName, server.Identity);
        sb.Append("---死亡排行榜---\n");
        var index = (currentPage - 1) * PAGE_SIZE + 1;
        foreach (var r in result.Ranking.Skip((currentPage - 1) * PAGE_SIZE).Take(PAGE_SIZE))
        {
            sb.AppendFormat("{0}. {1}: {2}\n", index, r.PlayerName, r.DeathCount);
            index++;
        }

        sb.AppendFormat("---页: <{0}/{1}>---", currentPage, lastPageNum);
        await eventArgs.SourceGroup.SendGroupMessage(sb.ToString());
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class DeathRankingRespond
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public class DeathRankingRecord
        {
            public string PlayerName { get; set; }
            public int DeathCount { get; set; }
        }

        [JsonPropertyName("ranking")] public DeathRankingRecord[]? Ranking { get; set; }
    }
}