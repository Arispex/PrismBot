using Sora.Entities;


namespace PrismBot.SDK.Extensions;

public static class MessageContextExtension
{
    public static async Task<string[]> GetCommandArgsAsync(this MessageContext messageContext)
    {
        return messageContext.RawText.Split(" ");
    }
}