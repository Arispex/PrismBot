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

    public List<IGroupCommand> GetRegisteredGroupCommands()
    {
        return CommandManager.RegisteredGroupCommands;
    }
    
    public List<IPrivateCommand> GetRegisteredPrivateCommands()
    {
        return CommandManager.RegisteredPrivateCommands;
    }

    public List<EndPoint> GetRegisteredEndPoints()
    {
        return EndPointManager.RegisteredEndPoints;
    }
}