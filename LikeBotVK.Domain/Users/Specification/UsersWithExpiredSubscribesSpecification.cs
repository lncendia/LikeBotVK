using Ardalis.Specification;
using LikeBotVK.Domain.Users.Entities;

namespace LikeBotVK.Domain.Users.Specification;

public sealed class UsersWithExpiredSubscribesSpecification : Specification<User>
{
    public UsersWithExpiredSubscribesSpecification()
    {
        Query.Where(user => user.Subscribes.Any(s => s.IsExpired()));
    }
}