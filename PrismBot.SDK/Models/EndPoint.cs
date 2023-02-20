namespace PrismBot.SDK.Models;

public class EndPoint
{
    public Delegate Delegate;
    public string Path;

    public EndPoint(string path, Delegate function)
    {
        Path = path;
        Delegate = function;
    }
}