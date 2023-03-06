using PrismBot.SDK.Interfaces;

namespace PrismBot.SDK;

public abstract class Plugin
{
    public List<IGroupCommand> RegisteredGroupCommands { get; } = new();
    public List<IPrivateCommand> RegisteredPrivateCommands { get; } = new();
    public abstract string GetPluginName();
    public abstract string GetVersion();
    public abstract string GetAuthor();
    public abstract string GetDescription();

    public abstract void OnLoad();
}