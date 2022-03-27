namespace LikeBotVK.Application.Abstractions.WebServices;

public interface IProxyParser
{
    Task AddProxiesAsync(string proxyList);
}