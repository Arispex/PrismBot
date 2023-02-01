using PrismBot.SDK.Models;

namespace PrismBot.SDK.Static;

public static class EndPointManager
{
    public static List<EndPoint> RegisteredEndPoints = new List<EndPoint>();

    public static void RegisterEndPoint(EndPoint endPoint)
    {
        RegisteredEndPoints.Add(endPoint);
    }
}