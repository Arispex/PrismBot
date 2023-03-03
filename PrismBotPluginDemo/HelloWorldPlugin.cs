using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Static;
using Sora.EventArgs.SoraEvent;

namespace PrismBotPluginDemo;

// ReSharper disable once UnusedType.Global
public class HelloWorldPlugin : PrismBot.SDK.Plugin
{
    public override string GetPluginName() => "Hello World";
    public override string GetVersion() => "1.0";
    public override string GetAuthor() => "Somebody";
    public override string GetDescription() => "Hello World";

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

    public string GetPermission() => string.Empty;

    public Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs) =>
        Task.CompletedTask;

    public async Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs) =>
        await eventArgs.Reply("Hello World!!!");
}