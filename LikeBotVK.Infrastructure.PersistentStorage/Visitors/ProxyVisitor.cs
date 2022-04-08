using System.Linq.Expressions;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.Proxies.Specification;
using LikeBotVK.Domain.Proxies.Specification.Visitor;
using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Infrastructure.PersistentStorage.Models;

namespace LikeBotVK.Infrastructure.PersistentStorage.Visitors;

public class ProxyVisitor : BaseVisitor<ProxyModel, IProxySpecificationVisitor, Proxy>, IProxySpecificationVisitor
{
    protected override Expression<Func<ProxyModel, bool>> ConvertSpecToExpression(
        ISpecification<Proxy, IProxySpecificationVisitor> spec)
    {
        var visitor = new ProxyVisitor();
        spec.Accept(visitor);
        return visitor.Expr!;
    }

    public void Visit(FromHostSpecification specification) => Expr = model => model.Host == specification.HostName;
    public void Visit(FromPortSpecification specification) => Expr = model => model.Port == specification.Port;
}