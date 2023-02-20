namespace PrismBot.SDK.Exceptions;

/// <summary>
///     未提供token（缺少token参数）
/// </summary>
public class NotAuthorized : Exception
{
    public NotAuthorized()
    {
    }

    public NotAuthorized(string? message) : base(message)
    {
    }

    public NotAuthorized(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}