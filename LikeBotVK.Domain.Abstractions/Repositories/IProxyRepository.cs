using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.Proxies.Specification.Visitor;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IProxyRepository : IRepository<Proxy, int, IProxySpecificationVisitor>
{
}