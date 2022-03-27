using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData.Context;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.ApplicationData.Services;

public class UserDataRepository : IUserDataRepository
{
    private readonly ApplicationDbContext _context;

    public UserDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<UserData?> GetAsync(long id)
    {
        return _context.UsersData.Where(data => data.UserId == id).Select(data => new UserData()
        {
            UserId = data.UserId,
            CurrentVkId = data.CurrentVkId,
            IsAdmin = data.IsAdmin,
            IsBanned = data.IsBanned,
            State = data.State,
            ReferralId = data.ReferralId,
            BonusAccount = data.BonusAccount
        }).FirstOrDefaultAsync();
    }

    public async Task AddOrUpdateAsync(UserData data)
    {
        var userData = _context.UsersData.FirstOrDefault(data1 => data1.UserId == data.UserId);
        if (userData == null)
        {
            await _context.AddAsync(new Models.UserData()
            {
                UserId = data.UserId,
                CurrentVkId = data.CurrentVkId,
                IsAdmin = data.IsAdmin,
                IsBanned = data.IsBanned,
                State = data.State,
                ReferralId = data.ReferralId,
                BonusAccount = data.BonusAccount
            });
        }
        else
        {
            userData.UserId = data.UserId;
            userData.CurrentVkId = data.CurrentVkId;
            userData.IsAdmin = data.IsAdmin;
            userData.IsBanned = data.IsBanned;
            userData.State = data.State;
            userData.ReferralId = data.ReferralId;
            userData.BonusAccount = data.BonusAccount;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var data = _context.UsersData.FirstOrDefault(data => data.UserId == id);
        if (data == null) return;
        _context.Remove(data);
        await _context.SaveChangesAsync();
    }
}