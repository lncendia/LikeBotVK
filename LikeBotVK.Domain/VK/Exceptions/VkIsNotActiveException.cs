using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.VK.Exceptions;

public sealed class VkNotActiveException : Exception
{
    public VkNotActiveException(Vk vk) : base($"Vk {vk.Id} is not active.")
    {
    }
}