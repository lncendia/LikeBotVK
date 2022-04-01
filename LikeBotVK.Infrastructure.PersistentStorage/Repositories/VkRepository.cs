using AutoMapper;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Specifications;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Specification.Visitor;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
using LikeBotVK.Infrastructure.PersistentStorage.Visitors;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.PersistentStorage.Repositories;

public class VkRepository : IVkRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public VkRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public async Task AddAsync(Vk entity)
    {
        var vk = _mapper.Map<VkModel>(entity);
        await _context.AddAsync(vk);
        await _context.SaveChangesAsync();
        entity.Id = vk.Id;
    }

    public async Task AddRangeAsync(List<Vk> entities)
    {
        var vks = _mapper.Map<List<VkModel>>(entities);
        await _context.AddRangeAsync(vks);
        await _context.SaveChangesAsync();
        for (int i = 0; i < entities.Count; i++) entities[i].Id = vks[i].Id;
    }

    public Task UpdateAsync(Vk entity)
    {
        var model = _context.Vks.First(x => x.Id == entity.Id);
        _mapper.Map(entity, model);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Vk> entities)
    {
        var ids = entities.Select(vk => vk.Id);
        var vks = await _context.Vks.Where(vk => ids.Contains(vk.Id)).ToListAsync();
        foreach (var entity in entities)
            _mapper.Map(entity, vks.First(vkModel => vkModel.Id == entity.Id));
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(Vk entity)
    {
        _context.Remove(_context.Vks.First(vk => vk.Id == entity.Id));
        return _context.SaveChangesAsync();
    }

    public Task DeleteRangeAsync(List<Vk> entities)
    {
        var ids = entities.Select(vk => vk.Id);
        _context.RemoveRange(_context.Vks.Where(vk => ids.Contains(vk.Id)));
        return _context.SaveChangesAsync();
    }

    public async Task<Vk?> GetAsync(int id)
    {
        var vk = await _context.Vks.FirstOrDefaultAsync(model => model.Id == id);
        return vk == null ? null : _mapper.Map<VkModel, Vk>(vk);
    }

    public async Task<List<Vk>> FindAsync(ISpecification<Vk, IVkSpecificationVisitor>? specification, int? skip = null,
        int? take = null)
    {
        var query = _context.Vks.AsQueryable();
        if (specification != null)
        {
            var visitor = new VkVisitor();
            specification.Accept(visitor);
            if (visitor.Expr != null) query = query.Where(visitor.Expr);
        }

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return _mapper.Map<List<VkModel>, List<Vk>>(await query.ToListAsync());
    }

    public Task<int> CountAsync(ISpecification<Vk, IVkSpecificationVisitor>? specification)
    {
        var query = _context.Vks.AsQueryable();
        if (specification == null) return query.CountAsync();
        var visitor = new VkVisitor();
        specification.Accept(visitor);
        if (visitor.Expr != null) query = query.Where(visitor.Expr);

        return query.CountAsync();
    }

    private static IMapper GetMapper() =>
        new Mapper(new MapperConfiguration(expr => { expr.CreateMap<Vk, VkModel>().ReverseMap(); }));
}