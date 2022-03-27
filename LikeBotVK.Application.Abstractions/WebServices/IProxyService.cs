using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.WebServices;

public interface IProxyService
{
    Task AddRangeAsync(List<ProxyDto> proxies);
    Task DeleteAsync();
}