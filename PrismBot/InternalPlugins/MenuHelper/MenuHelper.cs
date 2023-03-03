using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.MenuHelper;

public class MenuHelper : Plugin
{
    public override string GetPluginName()
    {
        return "MenuHelper";
    }

    public override string GetVersion()
    {
        return "1.0.0";
    }

    public override string GetAuthor()
    {
        return "Qianyiovo";
    }

    public override string GetDescription()
    {
        return "菜单小助手";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterPrivateCommand(new PrivateMenu());
        CommandManager.RegisterGroupCommand(new GroupMenu());
    }
}