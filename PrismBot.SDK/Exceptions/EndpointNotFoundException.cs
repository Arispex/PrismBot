namespace PrismBot.SDK.Exceptions;

public class EndpointNotFoundException : Exception
{
    public EndpointNotFoundException()
    {
    }

    public EndpointNotFoundException(string? message) : base(message)
    {
    }

    public EndpointNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}