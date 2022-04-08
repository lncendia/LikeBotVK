using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.Services.WebServices;

public interface IProxyService
{
    Task AddProxiesAsync(string proxyList);
    Task<(List<ProxyDto>, int)> FindAsync(string? host, int? port, int page);
    Task<bool> DeleteAsync(int id);
}