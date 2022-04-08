using System.Linq.Expressions;
using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Specification;
using LikeBotVK.Domain.VK.Specification.Visitor;
using LikeBotVK.Infrastructure.PersistentStorage.Models;

namespace LikeBotVK.Infrastructure.PersistentStorage.Visitors;

public class VkVisitor : BaseVisitor<VkModel, IVkSpecificationVisitor, Vk>, IVkSpecificationVisitor
{
    protected override Expression<Func<VkModel, bool>> ConvertSpecToExpression(
        ISpecification<Vk, IVkSpecificationVisitor> spec)
    {
        var visitor = new VkVisitor();
        spec.Accept(visitor);
        return visitor.Expr!;
    }

    public void Visit(ActiveVksSpecification specification) =>
        Expr = model => !string.IsNullOrEmpty(model.AccessToken);

    public void Visit(UserVksSpecification specification) => Expr = model => model.UserId == specification.UserId;
}