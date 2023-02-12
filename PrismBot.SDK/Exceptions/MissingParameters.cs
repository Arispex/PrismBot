namespace PrismBot.SDK.Exceptions;

/// <summary>
/// REST API缺少参数
/// </summary>
public class MissingParameters: Exception
{
    public MissingParameters()
    {
    }

    public MissingParameters(string? message) : base(message)
    {
    }

    public MissingParameters(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}