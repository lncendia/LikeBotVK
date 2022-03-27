using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.WebServices;

namespace LikeBotVK.Application.Services.Services;

public class UserService:IUserService
{
    public Task GetAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task FindAsync(long? id, bool admin, bool banned)
    {
        throw new NotImplementedException();
    }

    public Task EditApplicationDataAsync(EditApplicationUserDto editDto)
    {
        throw new NotImplementedException();
    }

    public Task AddSubscribeAsync(long id, DateTime endOfSubscribe)
    {
        throw new NotImplementedException();
    }
}