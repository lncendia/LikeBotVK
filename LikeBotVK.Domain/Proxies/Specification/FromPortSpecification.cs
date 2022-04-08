using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.Proxies.Specification.Visitor;
using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Domain.Proxies.Specification;

public class FromPortSpecification : ISpecification<Proxy, IProxySpecificationVisitor>
{
    public FromPortSpecification(int port)
    {
        Port = port;
    }

    public int Port { get; }
    public bool IsSatisfiedBy(Proxy item) => item.Port == Port;

    public void Accept(IProxySpecificationVisitor visitor) => visitor.Visit(this);
}