using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.VK.Exceptions;

public class ErrorActiveVkException : Exception
{
    public ErrorActiveVkException(Vk vk, string message, Exception? innerEx = null) : base(
        $"Can't active VK {vk.Id} - {message}.", innerEx)
    {
    }
}