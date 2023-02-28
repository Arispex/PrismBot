using System.Net;
using GenHTTP.Api.Infrastructure;
using GenHTTP.Engine;
using GenHTTP.Modules.Functional;
using PrismBot.SDK.Models;
using YukariToolBox.LightLog;
using EndPoint = PrismBot.SDK.Models.EndPoint;

namespace PrismBot.SDK.Static;

public static class EndPointManager
{
    private static IServerHost? _host;

    public static List<EndPoint> RegisteredEndPoints = new();

    /// <summary>
    ///     注册一个http端点
    /// </summary>
    /// <param name="endPoint">需要注册的对象</param>
    public static void RegisterEndPoint(EndPoint endPoint)
    {
        RegisteredEndPoints.Add(endPoint);
    }

    public static void StartServer()
    {
        Log.Info(nameof(EndPointManager), "正在启动 GenHttp...");
        _host ??= Host.Create()
            .Console()
            .Bind(IPAddress.Any, Config.Instance.GenHttpPort)
            .Handler(RegisteredEndPoints.Aggregate(Inline.Create(), (builder, ep) => builder.Get(ep.Path, ep.Delegate)))
            .Start();
        Log.Info(nameof(EndPointManager), $"GenHttp 正在运行 [0.0.0.0:{Config.Instance.GenHttpPort}]");
    }

    public static void StopServer()
    {
        Log.Info(nameof(EndPointManager), "正在停止 GenHttp...");
        _host?.Stop();
        _host = null;
        Log.Info(nameof(EndPointManager), "GenHttp 停止服务");
    }
}