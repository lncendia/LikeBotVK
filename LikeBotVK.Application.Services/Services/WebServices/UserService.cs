using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Application.Abstractions.Services.WebServices;
using LikeBotVK.Domain.Abstractions.Repositories;

namespace LikeBotVK.Application.Services.Services.WebServices;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDataUnitOfWork _applicationDataUnitOfWork;

    public UserService(IUnitOfWork unitOfWork, IApplicationDataUnitOfWork applicationDataUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _applicationDataUnitOfWork = applicationDataUnitOfWork;
    }

    public async Task<(List<UserDto>, int)> FindAsync(long? userId, int page)
    {
        List<UserDto> users;
        if (userId.HasValue)
        {
            users = new List<UserDto>();
            var user = await _applicationDataUnitOfWork.UserDataRepository.Value.GetAsync(userId.Value);
            if (user != null) users.Add(new UserDto(user.UserId, user.IsAdmin, user.IsBanned));
            return (users, users.Count);
        }

        users = (await _applicationDataUnitOfWork.UserDataRepository.Value.FindAsync((page - 1) * 30, 30)).Select(x =>
            new UserDto(x.UserId, x.IsAdmin, x.IsBanned)).ToList();
        var count = await _applicationDataUnitOfWork.UserDataRepository.Value.CountAsync();
        return (users, count);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var userTask = _unitOfWork.UserRepository.Value.GetAsync(id);
        var userDataTask = _applicationDataUnitOfWork.UserDataRepository.Value.GetAsync(id);
        await Task.WhenAll(userTask, userDataTask);
        var user = userTask.Result;
        var userData = userDataTask.Result;
        if (user == null && userData == null) return false;
        var t1 = user == null ? Task.CompletedTask : _unitOfWork.UserRepository.Value.DeleteAsync(user);
        var t2 = userData == null
            ? Task.CompletedTask
            : _applicationDataUnitOfWork.UserDataRepository.Value.DeleteAsync(userData);
        await Task.WhenAll(t1, t2);
        return true;
    }

    public async Task<UserExtendedDto?> GetAsync(long id)
    {
        var user = await _applicationDataUnitOfWork.UserDataRepository.Value.GetAsync(id);
        if (user == null) return null;
        var userDto = new UserExtendedDto(user.UserId, user.IsAdmin, user.IsBanned, user.BonusAccount, user.State);
        userDto.AddSubscribes(user.Subscribes.Select(x => x.EndSubscribe));
        return userDto;
    }

    public async Task<bool> EditApplicationDataAsync(EditApplicationUserDto editDto)
    {
        var user = await _applicationDataUnitOfWork.UserDataRepository.Value.GetAsync(editDto.Id);
        if (user == null) return false;
        user.State = editDto.State;
        user.IsAdmin = editDto.IsAdmin;
        user.IsBanned = editDto.IsBanned;
        user.BonusAccount = editDto.BonusAccount;
        await _applicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(user);
        return true;
    }

    public async Task<bool> AddSubscribeAsync(long id, DateTime endOfSubscribe)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        endOfSubscribe = TimeZoneInfo.ConvertTimeToUtc(endOfSubscribe, tz);
        if (endOfSubscribe <= DateTime.UtcNow) return false;
        var user = await _applicationDataUnitOfWork.UserDataRepository.Value.GetAsync(id);
        if (user == null) return false;
        user.Subscribes.Add(new Subscribe(endOfSubscribe));
        await _applicationDataUnitOfWork.UserDataRepository.Value.AddOrUpdateAsync(user);
        return true;
    }
}