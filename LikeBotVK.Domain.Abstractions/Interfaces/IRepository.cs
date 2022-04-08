using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Domain.Abstractions.Interfaces;

public interface IRepository<T, in TK, out TX> where T : class where TX : ISpecificationVisitor<TX, T>
{
    Task AddAsync(T entity);
    Task AddRangeAsync(List<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(List<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(List<T> entities);
    Task<T?> GetAsync(TK id);
    Task<List<T>> FindAsync(ISpecification<T, TX>? specification, int? skip = null, int? take = null);
    Task<int> CountAsync(ISpecification<T, TX>? specification);
}