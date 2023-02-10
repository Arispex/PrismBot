namespace PrismBot.SDK.Exceptions;

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