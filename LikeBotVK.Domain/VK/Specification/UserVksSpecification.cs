using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Domain.VK.Specification.Visitor;

namespace LikeBotVK.Domain.VK.Specification;

public class UserVksSpecification : ISpecification<Vk, IVkSpecificationVisitor>
{
    public long UserId { get; }

    public UserVksSpecification(long userId)
    {
        UserId = userId;
    }

    public bool IsSatisfiedBy(Vk item) => item.UserId == UserId;

    public void Accept(IVkSpecificationVisitor visitor) => visitor.Visit(this);
}