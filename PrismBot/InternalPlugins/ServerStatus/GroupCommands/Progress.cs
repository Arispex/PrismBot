using System.Reflection;
using PrismBot.SDK.Data;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using Sora.Entities;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ServerStatus.GroupCommands;

public class Progress : IGroupCommand
{
    public string GetCommand()
    {
        return "进度";
    }

    public string GetPermission()
    {
        return "ss.progress";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length != 2)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：进度 <服务器标识符>");
            return;
        }
        var db = new BotDbContext();

        var server = await db.Servers.FindAsync(args[1]);
        if (server == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("不存在该服务器。");
            return;
        }

        var bossProgress = await server.GetServerProgressAsync();
        var bossStatus = bossProgress.Response;
        var backgroundImage =
            await Image.LoadAsync(Path.Combine(AppContext.BaseDirectory, "images", "progress_bg.png"));
        var x = 260;
        var y = 460;
        var count = 0;
        
        // 定义你的字体文件的路径
        var fontPath = Path.Combine(AppContext.BaseDirectory, "fonts", "Alibaba-PuHuiTi-Medium.otf");

        // 加载字体
        var fontCollection = new FontCollection();
        var fontFamily = fontCollection.Add(fontPath);
        var font = fontFamily.CreateFont(32, FontStyle.Regular);
        
        foreach (var value in GetBossStatusValues(bossStatus))
        {
            backgroundImage.Mutate(image => image.DrawText(value ? "已  击  败" : "未  击  败",
                font, value ? Color.Red : Color.Black, new PointF(x, y)));
            x += 265;
            if (x > 1670 && count == 0)
            {
                x = 260;
                y += 270;
                count ++;
            }
            else if (x > 1670 && count != 0)
            {
                x = 260;
                y += 210;
            }
        }

        await backgroundImage.SaveAsync(Path.Combine(AppContext.BaseDirectory, "imageCache", "progress.png"));
        await eventArgs.SourceGroup.SendGroupMessage(SoraSegment.Image(Path.Combine(AppContext.BaseDirectory,
            "imageCache", "progress.png")));
    }
    
    public List<bool> GetBossStatusValues(BossStatus bossStatus)
    {
        List<bool> values = new List<bool>();

        PropertyInfo[] properties = typeof(BossStatus).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            if (property.PropertyType == typeof(bool))
            {
                bool value = (bool)property.GetValue(bossStatus);
                values.Add(value);
            }
        }

        return values;
    }
}