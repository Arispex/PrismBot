using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Interfaces;

public interface ICommand
{
    bool Match(string type, BaseMessageEventArgs eventArgs);

    string GetPermission();

    string GetDescription();

    string GetUsage();
}