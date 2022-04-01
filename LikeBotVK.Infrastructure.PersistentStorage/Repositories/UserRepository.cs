using AutoMapper;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Users.Entities;
using LikeBotVK.Domain.Users.Specification.Visitor;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Models;
using LikeBotVK.Infrastructure.PersistentStorage.Visitors;
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

    public async Task AddAsync(User entity)
    {
        var user = _mapper.Map<User, UserModel>(entity);
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
        entity.Id = user.Id;
    }

    public async Task AddRangeAsync(List<User> entities)
    {
        var users = _mapper.Map<List<User>, List<UserModel>>(entities);
        await _context.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        for (int i = 0; i < entities.Count; i++) entities[i].Id = users[i].Id;
    }

    public Task UpdateAsync(User entity)
    {
        var model = _context.Users.First(x => x.Id == entity.Id);
        _mapper.Map(entity, model);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<User> entities)
    {
        var ids = entities.Select(user => user.Id);
        var users = await _context.Users.Where(user => ids.Contains(user.Id)).ToListAsync();
        foreach (var entity in entities)
            _mapper.Map(entity, users.First(userModel => userModel.Id == entity.Id));
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

    public async Task<User?> GetAsync(long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(model => model.Id == id);
        return user == null ? null : _mapper.Map<UserModel, User>(user);
    }

    public async Task<List<User>> FindAsync(
        Domain.Specifications.ISpecification<User, IUserSpecificationVisitor>? specification, int? skip = null,
        int? take = null)
    {
        var query = _context.Users.AsQueryable();
        if (specification != null)
        {
            var visitor = new UserVisitor();
            specification.Accept(visitor);
            if (visitor.Expr != null) query = query.Where(visitor.Expr);
        }

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return _mapper.Map<List<UserModel>, List<User>>(await query.ToListAsync());
    }

    public Task<int> CountAsync(Domain.Specifications.ISpecification<User, IUserSpecificationVisitor>? specification)
    {
        var query = _context.Users.AsQueryable();
        if (specification == null) return query.CountAsync();
        var visitor = new UserVisitor();
        specification.Accept(visitor);
        if (visitor.Expr != null) query = query.Where(visitor.Expr);

        return query.CountAsync();
    }

    private static IMapper GetMapper()
    {
        return new Mapper(new MapperConfiguration(expr => { expr.CreateMap<User, UserModel>().ReverseMap(); }));
    }
}