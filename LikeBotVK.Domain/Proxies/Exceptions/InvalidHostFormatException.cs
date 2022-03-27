namespace LikeBotVK.Domain.Proxies.Exceptions;

public class InvalidHostFormatException : Exception
{
    public InvalidHostFormatException(string host) : base(
        $"The host {host} has an incorrect format.")
    {
    }
}