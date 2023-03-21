namespace PrismBot.SDK.Exceptions;

/// <summary>
///     未提供token（缺少token参数）
/// </summary>
public class NotAuthorizedException : Exception
{
    public NotAuthorizedException()
    {
    }

    public NotAuthorizedException(string? message) : base(message)
    {
    }

    public NotAuthorizedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}