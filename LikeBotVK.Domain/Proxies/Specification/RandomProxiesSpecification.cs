using LikeBotVK.Domain.Proxies.Entities;
using Ardalis.Specification;

namespace LikeBotVK.Domain.Proxies.Specification;

public sealed class RandomProxiesSpecification : Specification<Proxy>
{
    public RandomProxiesSpecification(int count)
    {
        Query.OrderBy(_ => Guid.NewGuid()).Take(count);
    }
}