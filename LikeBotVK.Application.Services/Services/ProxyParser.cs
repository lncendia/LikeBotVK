using System.Net;
using LikeBotVK.Application.Abstractions.WebServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Proxies.Entities;

namespace LikeBotVK.Application.Services.Services;

public class ProxyParser : IProxyParser
{
    private readonly IUnitOfWork _unitOfWork;

    public ProxyParser(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task AddProxiesAsync(string proxyList)
    {
        var proxyArray = proxyList.Split(Environment.NewLine);
        var list = proxyArray.Select(proxyString => proxyString.Split(":", 4))
            .Select(data => new Proxy(data[0], int.Parse(data[1]), data[2], data[3])).ToList();

        return _unitOfWork.ProxyRepository.Value.AddRangeAsync(list);
    }
}