using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.Proxies.Specification.Visitor;
using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Domain.Proxies.Specification;

public class FromHostSpecification : ISpecification<Proxy, IProxySpecificationVisitor>
{
    public FromHostSpecification(string hostName)
    {
        HostName = hostName;
    }

    public string HostName { get; }
    public bool IsSatisfiedBy(Proxy item) => item.Host == HostName;

    public void Accept(IProxySpecificationVisitor visitor) => visitor.Visit(this);
}