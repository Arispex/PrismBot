using Sora.Interfaces;

namespace PrismBot.SDK.Singletons;

public sealed class SoraServiceSingleton
{
    private static SoraServiceSingleton? _instance = null;
    private static readonly object Padlock = new object();
    public ISoraService? SoraService { get; set; }
    
    private SoraServiceSingleton() {}
    
    public static SoraServiceSingleton Instance
    {
        get
        {
            lock (Padlock)
            {
                if (_instance == null)
                {
                    _instance = new SoraServiceSingleton();
                }
                return _instance;
            }
        }
    }
}