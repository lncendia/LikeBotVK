namespace LikeBotVK.Application.Abstractions.Repositories;

public interface IBaseRepository<T, TK> where T : class
{
    Task<T?> GetAsync(TK id);
    Task AddOrUpdateAsync(T data);
    Task<List<T>> FindAsync(int? skip, int? take);
    Task<int> CountAsync();
    Task DeleteAsync(T data);
}