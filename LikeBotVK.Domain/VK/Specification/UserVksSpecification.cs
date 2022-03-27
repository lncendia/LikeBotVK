using LikeBotVK.Domain.VK.Entities;
using Ardalis.Specification;

namespace LikeBotVK.Domain.VK.Specification;

public class UserVksSpecification : Specification<Vk>
{
    public UserVksSpecification(long userId)
    {
        Query.Where(vk => vk.UserId == userId);
    }
}