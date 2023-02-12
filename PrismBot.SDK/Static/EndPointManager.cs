using PrismBot.SDK.Models;

namespace PrismBot.SDK.Static;

public static class EndPointManager
{
    public static List<EndPoint> RegisteredEndPoints = new List<EndPoint>();

    /// <summary>
    /// 注册一个http端点
    /// </summary>
    /// <param name="endPoint">需要注册的对象</param>
    public static void RegisterEndPoint(EndPoint endPoint)
    {
        RegisteredEndPoints.Add(endPoint);
    }
}