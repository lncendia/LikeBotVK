using AutoMapper;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.Proxies.Specification.Visitor;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
using LikeBotVK.Infrastructure.PersistentStorage.Visitors;
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

    public async Task AddAsync(Proxy entity)
    {
        var proxy = _mapper.Map<ProxyModel>(entity);
        await _context.AddAsync(proxy);
        await _context.SaveChangesAsync();
        entity.Id = proxy.Id;
    }

    public async Task AddRangeAsync(List<Proxy> entities)
    {
        var proxies = _mapper.Map<List<ProxyModel>>(entities);
        await _context.AddRangeAsync(proxies);
        await _context.SaveChangesAsync();
        for (int i = 0; i < entities.Count; i++) entities[i].Id = proxies[i].Id;
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

    public async Task<Proxy?> GetAsync(int id)
    {
        var proxy = await _context.Proxies.FirstOrDefaultAsync(model => model.Id == id);
        return proxy == null ? null : _mapper.Map<ProxyModel, Proxy>(proxy);
    }

    public async Task<List<Proxy>> FindAsync(ISpecification<Proxy, IProxySpecificationVisitor>? specification,
        int? skip = null, int? take = null)
    {
        var query = _context.Proxies.AsQueryable();
        if (specification != null)
        {
            var visitor = new ProxyVisitor();
            specification.Accept(visitor);
            if (visitor.Expr != null) query = query.Where(visitor.Expr);
        }

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return _mapper.Map<List<ProxyModel>, List<Proxy>>(await query.ToListAsync());
    }

    public Task<int> CountAsync(ISpecification<Proxy, IProxySpecificationVisitor>? specification)
    {
        var query = _context.Proxies.AsQueryable();
        if (specification == null) return query.CountAsync();
        var visitor = new ProxyVisitor();
        specification.Accept(visitor);
        if (visitor.Expr != null) query = query.Where(visitor.Expr);

        return query.CountAsync();
    }

    private static IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr => { expr.CreateMap<Proxy, ProxyModel>().ReverseMap(); }));
    }
}