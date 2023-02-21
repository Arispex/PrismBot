using Sora.Entities;

namespace PrismBot.SDK.Extensions;

public static class MessageContextExtension
{
    /// <summary>
    ///     获取消息命令参数
    /// </summary>
    /// <param name="messageContext"></param>
    /// <returns></returns>
    public static string[] GetCommandArgs(this MessageContext messageContext)
    {
        return messageContext.RawText.Trim().Split(" ");
    }
}