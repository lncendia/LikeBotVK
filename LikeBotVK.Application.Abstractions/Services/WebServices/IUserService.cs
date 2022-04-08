using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.Services.WebServices;

public interface IUserService
{
    Task<UserExtendedDto?> GetAsync(long id);
    Task<(List<UserDto>, int)> FindAsync(long? userId, int page);
    Task<bool> DeleteAsync(long id);
    Task<bool> EditApplicationDataAsync(EditApplicationUserDto editDto);
    Task<bool> AddSubscribeAsync(long id, DateTime endOfSubscribe);
}