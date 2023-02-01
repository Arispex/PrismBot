using PrismBot.SDK.Data;
using Sora.Entities;

namespace PrismBot.SDK.Extensions;

public static class UserExtension
{
    public static async Task<bool> IsGuest(this User user)
    {
        var botDbContext = new BotDbContext();
        var player = await botDbContext.Players.FindAsync(user.Id);
        if (player == null)
        {
            return true;
        }

        return false;
    }
}