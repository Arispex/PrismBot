using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Interfaces;

public interface IPrivateCommand: ICommand
{
    Task OnPermissionDenied(string type, PrivateMessageEventArgs eventArgs);
    Task OnPermissionGranted(string type, PrivateMessageEventArgs eventArgs);
}