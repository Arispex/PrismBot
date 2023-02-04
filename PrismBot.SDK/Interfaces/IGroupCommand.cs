using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Interfaces;

public interface IGroupCommand : ICommand
{
    Task OnPermissionDeniedAsync(string type, GroupMessageEventArgs eventArgs);
    Task OnPermissionGrantedAsync(string type, GroupMessageEventArgs eventArgs);
}