using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.WebServices;

public interface IUserService
{
    Task GetAsync(long id);
    Task DeleteAsync(long id);
    Task FindAsync(long? id, bool admin, bool banned);
    Task EditApplicationDataAsync(EditApplicationUserDto editDto);
    Task AddSubscribeAsync(long id, DateTime endOfSubscribe);
}