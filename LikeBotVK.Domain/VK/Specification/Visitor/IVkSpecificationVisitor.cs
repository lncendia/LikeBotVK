using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.VK.Specification.Visitor;

public interface IVkSpecificationVisitor : ISpecificationVisitor<IVkSpecificationVisitor, Vk>
{
    void Visit(ActiveVkSpecification specification);
    void Visit(UserVkSpecification specification);
    void Visit(VkFromUsernameSpecification specification);
}