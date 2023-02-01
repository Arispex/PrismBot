using Sora.EventArgs.SoraEvent;

namespace PrismBot.SDK.Interfaces;

public interface IGroupCommand : ICommand
{
    Task OnPermissionDenied(string type, GroupMessageEventArgs eventArgs);
    Task OnPermissionGranted(string type, GroupMessageEventArgs eventArgs);
}