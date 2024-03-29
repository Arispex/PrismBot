using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Data;
using PrismBot.SDK.Exceptions;
using PrismBot.SDK.Extensions;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using Sora.Entities.Segment;
using Sora.EventArgs.SoraEvent;

namespace PrismBot.InternalPlugins.ServerStatus.GroupCommands;

public class UserBag : IGroupCommand
{
    public string GetCommand()
    {
        return "用户背包";
    }

    public string GetPermission()
    {
        return "ss.userbag";
    }

    public async Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        await eventArgs.SendDefaultPermissionDeniedMessageAsync();
    }

    public async Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs)
    {
        var args = eventArgs.Message.GetCommandArgs();
        if (args.Length != 3)
        {
            await eventArgs.SourceGroup.SendGroupMessage("您输入的参数不符合要求。请参考以下语法进行输入：用户背包 <服务器标识符> <用户昵称/QQ/At>");
            return;
        }
        //获取玩家
        var db = new BotDbContext();
        Player? player = null;
        //判断有无at
        if (eventArgs.Message.GetAllAtList().Count() == 1)
            player = await db.Players.FirstOrDefaultAsync(x => x.QQ == eventArgs.Message.GetAllAtList().First());

        if (player == null)
        {
            //判断是否为QQ号
            if (long.TryParse(args[2], out var qq))
            {
                player = await db.Players.FirstOrDefaultAsync(x => x.QQ == qq) ?? await db.Players.FirstOrDefaultAsync(x => x.UserName == args[2]);
            }
            else
                //判断是否为角色昵称
                player = await db.Players.FirstOrDefaultAsync(x => x.UserName == args[2]);
        }

        if (player == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("未找到该用户。");
            return;
        }
        var server = await db.Servers.FindAsync(args[1]);
        if (server == null)
        {
            await eventArgs.SourceGroup.SendGroupMessage("不存在该服务器。");
            return;
        }
        PlayerInfo playerInfo;
        try
        {
            playerInfo = await server.GetPlayerInfoAsync(player);
        }
        catch (EndpointNotFoundException)
        {
            await eventArgs.SourceGroup.SendGroupMessage("未安装TShock适配插件。");
            return;
        }
        catch (InvalidTokenException)
        {
            await eventArgs.SourceGroup.SendGroupMessage("REST API Token无效。");
            return;
        }
        catch (MissingParametersException)
        {
            await eventArgs.SourceGroup.SendGroupMessage("该玩家未在服务器中注册。");
            return;
        }
        catch (HttpRequestException)
        {
            await eventArgs.SourceGroup.SendGroupMessage("服务器连接失败。");
            return;
        }
        
        var backgroundImage = await Image.LoadAsync(Path.Combine(AppContext.BaseDirectory, "images", "background.png"));
        var Bx = 10;
        var By = 10;
        foreach (var item in playerInfo.Inventory)
        {
            var frameImage = await Image.LoadAsync(Path.Combine(AppContext.BaseDirectory, "images", "frame.png"));
            var itemImage = File.Exists(Path.Combine(AppContext.BaseDirectory, "images", "items", $"Item_{item.NetId}.png")) ? 
                await Image.LoadAsync(Path.Combine(AppContext.BaseDirectory, "images", "items", $"Item_{item.NetId}.png")) : await Image.LoadAsync(Path.Combine(AppContext.BaseDirectory, "images", "items", "Item_-1.png"));
            if (item.Stack != 0)
            {
                frameImage.Mutate(x =>
                {
                    x.DrawImage(itemImage, new Point(
                        (frameImage.Size.Width - itemImage.Size.Width) / 2, (frameImage.Size.Height - itemImage.Size.Height) / 2), opacity: 1);
                    x.DrawText(item.Stack.ToString(), SystemFonts.CreateFont("Arial", 16, FontStyle.Regular), Color.Black,
                        new PointF(35, 35));
                });
            }
            await frameImage.SaveAsync(Path.Combine(AppContext.BaseDirectory, "imageCache", "frame.png"));
            backgroundImage.Mutate(x => x.DrawImage(frameImage, new Point(Bx, By), opacity: 1));
            Bx += 70;
            if (Bx > 1700)
            {
                Bx = 10;
                By += 70;
            }
        }
        await backgroundImage.SaveAsync(Path.Combine(AppContext.BaseDirectory, "imageCache", "background.png"));
        await eventArgs.SourceGroup.SendGroupMessage(SoraSegment.Image(Path.Combine(AppContext.BaseDirectory, "imageCache", "background.png")));
    }
}