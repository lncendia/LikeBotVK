using Ardalis.Specification;
using LikeBotVK.Domain.Users.Entities;

namespace LikeBotVK.Domain.Users.Specification;

public sealed class AllUsersSpecification : Specification<User>
{
    public AllUsersSpecification(int? skip = null, int? take = null)
    {
        if (skip.HasValue) Query.Skip(skip.Value);
        if (take.HasValue) Query.Take(take.Value);
    }
}