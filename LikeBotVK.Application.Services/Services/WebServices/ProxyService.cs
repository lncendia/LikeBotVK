using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Services.WebServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.Proxies.Specification;
using LikeBotVK.Domain.Proxies.Specification.Visitor;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Application.Services.Services.WebServices;

public class ProxyService : IProxyService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProxyService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<(List<ProxyDto>, int)> FindAsync(string? host, int? port, int page)
    {
        var spec = GetSpecification(host, port);
        return (
            (await _unitOfWork.ProxyRepository.Value.FindAsync(spec, (page - 1) * 30, 30))
            .Select(x => new ProxyDto(x.Host, x.Port, x.Login, x.Password, x.Id)).ToList(),
            await _unitOfWork.ProxyRepository.Value.CountAsync(spec));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var proxy = await _unitOfWork.ProxyRepository.Value.GetAsync(id);
        if (proxy == null) return false;
        await _unitOfWork.ProxyRepository.Value.DeleteAsync(proxy);
        return true;
    }

    public Task AddProxiesAsync(string proxyList)
    {
        var proxyArray = proxyList.Split(Environment.NewLine);
        var list = proxyArray.Select(proxyString => proxyString.Split(":", 4))
            .Select(data => new Proxy(data[0], int.Parse(data[1]), data[2], data[3])).ToList();

        return _unitOfWork.ProxyRepository.Value.AddRangeAsync(list);
    }

    private static ISpecification<Proxy, IProxySpecificationVisitor>? GetSpecification(string? host, int? port)
    {
        ISpecification<Proxy, IProxySpecificationVisitor>? specification = null;
        if (!string.IsNullOrEmpty(host)) specification = new FromHostSpecification(host);

        if (!port.HasValue) return specification;
        if (specification != null)
            specification =
                new AndSpecification<Proxy, IProxySpecificationVisitor>(specification,
                    new FromPortSpecification(port.Value));
        else specification = new FromPortSpecification(port.Value);

        return specification;
    }
}