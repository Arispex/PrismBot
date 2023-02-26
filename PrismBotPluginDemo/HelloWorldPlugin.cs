using PrismBot.SDK.Interfaces;
using PrismBot.SDK.Static;
using Sora.EventArgs.SoraEvent;

namespace PrismBotPluginDemo;

// ReSharper disable once UnusedType.Global
public class HelloWorldPlugin : PrismBot.SDK.Plugin
{
    public override string Name => "Hello World";
    public override string Author => "Somebody";
    public override string Version => "1.0";
    public override string Description => "Hello World";

    public override void OnLoad()
    {
        CommandManager.RegisterPrivateCommand(new HelloWorldReplyer());
    }
}

public class HelloWorldReplyer : IPrivateCommand
{
    public bool Match(string type, BaseMessageEventArgs eventArgs) =>
        eventArgs.Message.RawText.Contains("Hello World");

    public string GetPermission() => string.Empty;

    public Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs) =>
        Task.CompletedTask;

    public async Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs) =>
        await eventArgs.Reply("Hello World!!!");
}