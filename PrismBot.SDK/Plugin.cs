namespace PrismBot.SDK;

public abstract class Plugin
{
    public abstract string GetPluginName();
    public abstract string GetVersion();
    public abstract string GetAuthor();
    public abstract string GetDescription();

    public abstract void OnLoad();
}