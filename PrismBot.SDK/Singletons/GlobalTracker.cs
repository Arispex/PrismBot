using Sora.Interfaces;

namespace PrismBot.SDK.Singletons;

public static class GlobalTracker
{
    public static ISoraService? SoraService { get; set; }
}