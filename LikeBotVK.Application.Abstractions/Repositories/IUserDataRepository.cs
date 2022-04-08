using LikeBotVK.Application.Abstractions.ApplicationData;

namespace LikeBotVK.Application.Abstractions.Repositories;

public interface IUserDataRepository : IBaseRepository<UserData, long>
{
    Task<List<UserData>> GetUsersWithExpiredSubscribesAsync();
}