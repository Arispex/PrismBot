namespace PrismBot.SDK.Exceptions;

/// <summary>
///     REST API缺少参数
/// </summary>
public class MissingParametersException : Exception
{
    public MissingParametersException()
    {
    }

    public MissingParametersException(string? message) : base(message)
    {
    }

    public MissingParametersException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}