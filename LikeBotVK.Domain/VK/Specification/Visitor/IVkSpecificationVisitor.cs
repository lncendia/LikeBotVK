using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.VK.Specification.Visitor;

public interface IVkSpecificationVisitor : ISpecificationVisitor<IVkSpecificationVisitor, Vk>
{
    void Visit(ActiveVksSpecification specification);
    void Visit(UserVksSpecification specification);
}