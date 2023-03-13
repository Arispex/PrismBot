using PrismBot.InternalPlugins.GroupManager.GroupCommands;
using PrismBot.SDK;
using PrismBot.SDK.Static;

namespace PrismBot.InternalPlugins.GroupManager;

public class GroupManager : Plugin
{
    public override string GetPluginName()
    {
        return "GroupManager";
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
        return "组管理插件";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterGroupCommand(this, new GroupList());
        CommandManager.RegisterGroupCommand(this, new GroupInformation());
        CommandManager.RegisterGroupCommand(this, new AddGroup());
        CommandManager.RegisterGroupCommand(this, new RemoveGroup());
    }
}