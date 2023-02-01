namespace PrismBot.SDK.Models;

public class PlayerFactory
{
    public static Player CreatePlayer(long qq, string userName, Group group)
    {
        return new Player
        {
            QQ = qq,
            UserName = userName,
            Group = group,
            Permissions = string.Empty,
            Coins = 0
        };
    }
}