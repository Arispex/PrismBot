namespace PrismBot.SDK.Models;

public class EndPoint
{
    public string Path;
    public Delegate Delegate;

    public EndPoint(string path, Delegate function)
    {
        Path = path;
        Delegate = function;
    }
}