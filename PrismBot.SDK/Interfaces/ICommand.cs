using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Interfaces;

public interface ICommand
{
    string GetCommand();

    string GetPermission();
}