using System.ComponentModel.DataAnnotations;

namespace PrismBot.SDK.Models;

public class Server
{
    public string ServerName { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Token { get; set; }

    [Key] public string Identity { get; set; }
}