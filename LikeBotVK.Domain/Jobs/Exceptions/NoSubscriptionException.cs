using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.Jobs.Exceptions;

public class NoSubscriptionException : Exception
{
    public NoSubscriptionException() : base($"There is no free subscription to create a job.")
    {
    }
}