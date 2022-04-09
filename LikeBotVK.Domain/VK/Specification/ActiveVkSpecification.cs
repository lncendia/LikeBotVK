using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.VK.Specification.Visitor;

namespace LikeBotVK.Domain.VK.Specification;

public sealed class ActiveVkSpecification : ISpecification<Vk, IVkSpecificationVisitor>
{
    public bool IsSatisfiedBy(Vk item) => item.IsActive();
    public void Accept(IVkSpecificationVisitor visitor) => visitor.Visit(this);
}