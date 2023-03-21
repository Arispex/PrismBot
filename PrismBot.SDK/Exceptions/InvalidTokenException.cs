namespace PrismBot.SDK.Exceptions;

/// <summary>
///     REST API令牌(Token)错误
/// </summary>
public class InvalidTokenException : Exception
{
    public InvalidTokenException()
    {
    }

    public InvalidTokenException(string? message) : base(message)
    {
    }

    public InvalidTokenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}