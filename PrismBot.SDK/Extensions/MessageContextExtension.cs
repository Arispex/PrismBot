using Sora.Entities;


namespace PrismBot.SDK.Extensions;

public static class MessageContextExtension
{
    public static string[] GetCommandArgs(this MessageContext messageContext)
    {
        return messageContext.RawText.Split(" ");
    }
}