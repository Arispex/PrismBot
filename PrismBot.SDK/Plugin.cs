using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using PrismBot.SDK.Static;

namespace PrismBot.SDK;

public abstract class Plugin
{
    public abstract string GetPluginName();
    public abstract string GetVersion();
    public abstract string GetAuthor();
    public abstract string GetDescription();
    public abstract void OnLoad();
    /// <summary>
    /// 获取已注册的群命令
    /// </summary>
    /// <returns>群命令列表</returns>
    public virtual List<IGroupCommand> GetRegisteredGroupCommands()
    {
        return new();
    }
    /// <summary>
    /// 获取已注册的私聊命令
    /// </summary>
    /// <returns>私聊命名列表</returns>
    public virtual List<IPrivateCommand> GetRegisteredPrivateCommands()
    {
        return new();
    }

    /// <summary>
    /// 获取已注册的端点
    /// </summary>
    /// <returns>端点列表</returns>
    public virtual List<EndPoint> GetRegisteredEndPoints()
    {
        return new();
    }
}