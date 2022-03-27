using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Users.Entities;
using LikeBotVK.Domain.Users.ValueObjects;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.PersistentStorage.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
        _mapper = GetMapper();
    }

    public Task<List<User>> FindAsync(ISpecification<User> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Users.ProjectTo<User>(_mapper.ConfigurationProvider), specification).ToListAsync();

    public Task<int> CountAsync(ISpecification<User> specification) =>
        SpecificationEvaluator.Default
            .GetQuery(_context.Users.ProjectTo<User>(_mapper.ConfigurationProvider), specification).CountAsync();

    private UserModel AddMap(User entity)
    {
        var user = _mapper.Map<UserModel>(entity);
        user.Subscribes = _mapper.Map<List<Subscribe>, List<SubscribeModel>>(entity.Subscribes);
        return user;
    }

    private void UpdateMap(User entity, UserModel destination)
    {
        _mapper.Map(entity, destination);
        var subscribes = _mapper.Map<List<Subscribe>, List<SubscribeModel>>(entity.Subscribes);
        if (destination.Subscribes.SequenceEqual(subscribes)) return;
        destination.Subscribes.Clear();
        destination.Subscribes.AddRange(subscribes);
    }

    public async Task AddAsync(User entity)
    {
        var user = AddMap(entity);
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<User> entities)
    {
        var users = entities.Select(AddMap);
        await _context.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(User entity)
    {
        var model = _context.Users.Include(u => u.Subscribes).First(x => x.Id == entity.Id);
        UpdateMap(entity, model);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<User> entities)
    {
        var ids = entities.Select(user => user.Id);
        var users = await _context.Users.Include(u => u.Subscribes).Where(user => ids.Contains(user.Id)).ToListAsync();
        foreach (var entity in entities)
            UpdateMap(entity, users.First(userModel => userModel.Id == entity.Id));
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(User entity)
    {
        _context.Remove(_context.Users.First(user => user.Id == entity.Id));
        return _context.SaveChangesAsync();
    }

    public Task DeleteRangeAsync(List<User> entities)
    {
        var ids = entities.Select(user => user.Id);
        _context.RemoveRange(_context.Users.Where(user => ids.Contains(user.Id)));
        return _context.SaveChangesAsync();
    }

    public Task<User?> GetAsync(long id) => _context.Users.ProjectTo<User>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(model => model.Id == id);

    private static IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr =>
        {
            expr.CreateMap<User, UserModel>().ForMember(model => model.Subscribes, expression => expression.Ignore());
            expr.CreateMap<UserModel, User>();
            expr.CreateMap<Subscribe, SubscribeModel>().ReverseMap();
        }));
    }
}