using Microsoft.EntityFrameworkCore;
using PrismBot.SDK.Models;

namespace PrismBot.SDK.Data;

public class BotDbContext : DbContext
{
    public string DbPath { get; init; } = Path.Combine(AppContext.BaseDirectory, "bot.db");

    public DbSet<Player> Players { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Server> Servers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}