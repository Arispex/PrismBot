using Sora.Interfaces;

namespace PrismBot.SDK.Singletons;

public sealed class SoraServiceSingleton
{
    private static SoraServiceSingleton? _instance;
    private static readonly object Padlock = new();

    private SoraServiceSingleton()
    {
    }

    public ISoraService? SoraService { get; set; }

    public static SoraServiceSingleton Instance
    {
        get
        {
            lock (Padlock)
            {
                if (_instance == null) _instance = new SoraServiceSingleton();
                return _instance;
            }
        }
    }
}