using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Domain.Proxies.Specification.Visitor;

public interface IProxySpecificationVisitor : ISpecificationVisitor<IProxySpecificationVisitor, Proxy>
{
    void Visit(FromHostSpecification specification);
    void Visit(FromPortSpecification specification);
}