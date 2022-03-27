using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.Repositories;

public interface IUserDataRepository
{
    Task<UserData?> GetAsync(long id);
    Task AddOrUpdateAsync(UserData data);
    Task DeleteAsync(long id);
}