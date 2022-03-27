using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.VK.Exceptions;

public class NoSubscriptionException : Exception
{
    public NoSubscriptionException() : base($"There is no free subscription to add an account.")
    {
    }
}