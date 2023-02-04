using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Interfaces;

public interface IPrivateCommand: ICommand
{
    Task OnPermissionDeniedAsync(string type, PrivateMessageEventArgs eventArgs);
    Task OnPermissionGrantedAsync(string type, PrivateMessageEventArgs eventArgs);
}