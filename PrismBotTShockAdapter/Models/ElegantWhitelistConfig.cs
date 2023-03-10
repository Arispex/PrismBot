namespace PrismBotTShockAdapter.Models;

public class ElegantWhitelistConfig
{
    public string AccountFrozenMessage = "你的账号已被冻结，请联系服务器管理员";
    public string ConnectionErrorMessage = "无法连接到白名单服务器，请联系服务器管理员";
    public string Host = "127.0.0.1";
    public string NotInWhitelistMessage = "你还没有添加白名单，请在群内发送“添加白名单 <角色昵称>”来添加白名单";
    public int Port = 8081;
}