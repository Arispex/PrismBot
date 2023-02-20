using PrismBot.SDK.Interfaces;

namespace PrismBot.SDK.Static;

public static class CommandManager
{
    public static List<IGroupCommand> RegisteredGroupCommands = new();
    public static List<IPrivateCommand> RegisteredPrivateCommands = new();

    /// <summary>
    ///     注册群命令
    /// </summary>
    /// <param name="command">需要注册的对象</param>
    public static void RegisterGroupCommand(IGroupCommand command)
    {
        RegisteredGroupCommands.Add(command);
    }

    /// <summary>
    ///     注册私聊命令
    /// </summary>
    /// <param name="command">需要注册的对象</param>
    public static void RegisterPrivateCommand(IPrivateCommand command)
    {
        RegisteredPrivateCommands.Add(command);
    }
}