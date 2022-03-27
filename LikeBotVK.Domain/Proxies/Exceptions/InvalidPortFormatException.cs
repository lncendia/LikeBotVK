namespace LikeBotVK.Domain.Proxies.Exceptions;

public class InvalidPortFormatException : Exception
{
    public InvalidPortFormatException(int port) : base(
        $"The port {port} has an incorrect format. It should be in the range from 0 to 65536")
    {
    }
}