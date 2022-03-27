using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
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

    public Task<List<Vk>> FindAsync(ISpecification<Vk> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Vks.ProjectTo<Vk>(_mapper.ConfigurationProvider), specification).ToListAsync();

    public Task<int> CountAsync(ISpecification<Vk> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Vks.ProjectTo<Vk>(_mapper.ConfigurationProvider), specification).CountAsync();

    public async Task AddAsync(Vk entity)
    {
        var vk = _mapper.Map<VkModel>(entity);
        await _context.AddAsync(vk);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<Vk> entities)
    {
        var vks = _mapper.Map<List<VkModel>>(entities);
        await _context.AddRangeAsync(vks);
        await _context.SaveChangesAsync();
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

    public Task<Vk?> GetAsync(int id) => _context.Vks.ProjectTo<Vk>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(model => model.Id == id);

    private static IMapper GetMapper() =>
        new Mapper(new MapperConfiguration(expr => { expr.CreateMap<Vk, VkModel>().ReverseMap(); }));
}