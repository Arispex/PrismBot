using PrismBot.SDK;
using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Static;
using Sora.EventArgs.SoraEvent;

namespace PrismBotPluginDemo;

// ReSharper disable once UnusedType.Global
public class HelloWorldPlugin : Plugin
{
    public override string GetPluginName()
    {
        return "Hello World";
    }

    public override string GetVersion()
    {
        return "1.0";
    }

    public override string GetAuthor()
    {
        return "Somebody";
    }

    public override string GetDescription()
    {
        return "Hello World";
    }

    public override void OnLoad()
    {
        CommandManager.RegisterPrivateCommand(new HelloWorldReplyer());
    }
}

public class HelloWorldReplyer : IPrivateCommand
{
    public string GetCommand()
    {
        return "Hello World";
    }

    public string GetPermission()
    {
        return string.Empty;
    }

    public Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        return Task.CompletedTask;
    }

    public async Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs)
    {
        await eventArgs.Reply("Hello World!!!");
    }
}