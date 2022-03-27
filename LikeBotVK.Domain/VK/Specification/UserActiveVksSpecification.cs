using LikeBotVK.Domain.VK.Entities;
using Ardalis.Specification;

namespace LikeBotVK.Domain.VK.Specification;

public sealed class UserActiveVksSpecification : UserVksSpecification
{
    public UserActiveVksSpecification(long userId) : base(userId)
    {
        Query.Where(vk => !string.IsNullOrEmpty(vk.AccessToken));
    }
}