using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Application.Abstractions.Exceptions;

public class ErrorActiveVkException : Exception
{
    public ErrorActiveVkException(Vk vk, string message, Exception? innerEx = null) : base(
        $"Can't active VK {vk.Id} - {message}.", innerEx)
    {
    }
}