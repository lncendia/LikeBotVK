using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.VK.Specification.Visitor;

namespace LikeBotVK.Domain.VK.Specification;

public class VkFromUsernameSpecification : ISpecification<Vk, IVkSpecificationVisitor>
{
    public string Username { get; }

    public VkFromUsernameSpecification(string username)
    {
        Username = username;
    }

    public bool IsSatisfiedBy(Vk item) => item.Username == Username;

    public void Accept(IVkSpecificationVisitor visitor) => visitor.Visit(this);
}