using Ardalis.Specification;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Users.Entities;

namespace LikeBotVK.Domain.Abstractions.Interfaces;

public interface IRepository<T, in TK> where T : class
{
    Task<List<T>> FindAsync(ISpecification<T> specification);
    Task<int> CountAsync(ISpecification<T> specification);
    Task AddAsync(T entity);
    Task AddRangeAsync(List<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(List<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(List<T> entities);
    Task<T?> GetAsync(TK id);
}