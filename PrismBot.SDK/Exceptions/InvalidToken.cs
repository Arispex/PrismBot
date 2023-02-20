namespace PrismBot.SDK.Exceptions;

/// <summary>
///     REST API令牌(Token)错误
/// </summary>
public class InvalidToken : Exception
{
    public InvalidToken()
    {
    }

    public InvalidToken(string? message) : base(message)
    {
    }

    public InvalidToken(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}