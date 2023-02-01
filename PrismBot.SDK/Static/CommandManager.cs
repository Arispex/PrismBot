using PrismBot.SDK.Interfaces;

namespace PrismBot.SDK.Static;

public static class CommandManager
{
    public static List<IGroupCommand> RegisteredGroupCommands = new();
    public static List<IPrivateCommand> RegisteredPrivateCommands = new();

    public static void RegisterGroupCommand(IGroupCommand command)
    {
        RegisteredGroupCommands.Add(command);
    }
    
    public static void RegisterPrivateCommand(IPrivateCommand command)
    {
        RegisteredPrivateCommands.Add(command);
    }
}