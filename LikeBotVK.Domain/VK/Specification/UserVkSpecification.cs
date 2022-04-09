using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.VK.Specification.Visitor;

namespace LikeBotVK.Domain.VK.Specification;

public class UserVkSpecification : ISpecification<Vk, IVkSpecificationVisitor>
{
    public long UserId { get; }

    public UserVkSpecification(long userId)
    {
        UserId = userId;
    }

    public bool IsSatisfiedBy(Vk item) => item.UserId == UserId;

    public void Accept(IVkSpecificationVisitor visitor) => visitor.Visit(this);
}