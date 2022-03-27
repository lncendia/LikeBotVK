using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.PersistentStorage.Repositories;

public class ProxyRepository : IProxyRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProxyRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public Task<List<Proxy>> FindAsync(ISpecification<Proxy> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Proxies.ProjectTo<Proxy>(_mapper.ConfigurationProvider), specification).ToListAsync();

    public Task<int> CountAsync(ISpecification<Proxy> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Proxies.ProjectTo<Proxy>(_mapper.ConfigurationProvider), specification).CountAsync();

    public async Task AddAsync(Proxy entity)
    {
        var proxy = _mapper.Map<ProxyModel>(entity);
        await _context.AddAsync(proxy);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Proxy> entities)
    {
        var proxies = _mapper.Map<List<ProxyModel>>(entities);
        await _context.AddRangeAsync(proxies);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(Proxy entity)
    {
        var model = _context.Proxies.First(x => x.Id == entity.Id);
        _mapper.Map(entity, model);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Proxy> entities)
    {
        var ids = entities.Select(proxy => proxy.Id);
        var proxies = await _context.Proxies.Where(proxy => ids.Contains(proxy.Id)).ToListAsync();
        foreach (var entity in entities)
            _mapper.Map(entity, proxies.First(proxyModel => proxyModel.Id == entity.Id));
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Proxy entity)
    {
        _context.Remove(_context.Proxies.First(proxy => proxy.Id == entity.Id));
        return _context.SaveChangesAsync();
    }

    public Task DeleteRangeAsync(List<Proxy> entities)
    {
        var ids = entities.Select(proxy => proxy.Id);
        _context.RemoveRange(_context.Proxies.Where(proxy => ids.Contains(proxy.Id)));
        return _context.SaveChangesAsync();
    }

    public Task<Proxy?> GetAsync(int id) => _context.Proxies.ProjectTo<Proxy>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(model => model.Id == id);

    private static IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr => { expr.CreateMap<Proxy, ProxyModel>().ReverseMap(); }));
    }
}