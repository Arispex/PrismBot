using Sora.Entities;


namespace PrismBot.SDK.Extensions;

public static class MessageContextExtension
{
    public static async Task<string[]> GetCommandArgs(this MessageContext messageContext)
    {
        return messageContext.RawText.Split(" ");
    }
}