using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Models;
using PrismBot.SDK.Static;

namespace PrismBot.SDK;

public abstract class Plugin
{
    public virtual string Name => "None";
    public virtual string Version => "1.0";
    public virtual string Author => "None";
    public virtual string Description => "None";

    public abstract void OnLoad();
}