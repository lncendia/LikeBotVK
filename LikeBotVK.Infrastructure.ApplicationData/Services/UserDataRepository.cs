using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData.Context;
using LikeBotVK.Infrastructure.ApplicationData.EqualityComparers;
using LikeBotVK.Infrastructure.ApplicationData.Models;
using Microsoft.EntityFrameworkCore;
using UserData = LikeBotVK.Application.Abstractions.ApplicationData.UserData;

namespace LikeBotVK.Infrastructure.ApplicationData.Services;

public class UserDataRepository : IUserDataRepository
{
    private readonly ApplicationDbContext _context;

    public UserDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private static UserData Map(Models.UserData userData)
    {
        var user = new UserData
        {
            UserId = userData.UserId,
            CurrentVkId = userData.CurrentVkId,
            IsAdmin = userData.IsAdmin,
            IsBanned = userData.IsBanned,
            State = userData.State,
            ReferralId = userData.ReferralId,
            BonusAccount = userData.BonusAccount,
            CurrentJobsId = userData.CurrentJobsId
        };
        foreach (var data in userData.Subscribes)
            user.Subscribes.Add(new Subscribe(data.EndSubscribe));
        return user;
    }

    private static void Map(UserData userData, Models.UserData destination)
    {
        destination.UserId = userData.UserId;
        destination.CurrentVkId = userData.CurrentVkId;
        destination.IsAdmin = userData.IsAdmin;
        destination.IsBanned = userData.IsBanned;
        destination.State = userData.State;
        destination.ReferralId = userData.ReferralId;
        destination.BonusAccount = userData.BonusAccount;
        destination.CurrentJobsId = userData.CurrentJobsId;
        var subscribes =
            userData.Subscribes.Select(x => new SubscribeData() {EndSubscribe = x.EndSubscribe}).ToList();
        if (subscribes.SequenceEqual(destination.Subscribes, new SubscribeEqualityComparer())) return;
        destination.Subscribes.Clear();
        destination.Subscribes.AddRange(subscribes);
    }

    public async Task<UserData?> GetAsync(long id)
    {
        var data = await _context.UsersData.Where(data => data.UserId == id).FirstOrDefaultAsync();
        return data == null ? null : Map(data);
    }

    public async Task AddOrUpdateAsync(UserData data)
    {
        var userData = _context.UsersData.FirstOrDefault(data1 => data1.UserId == data.UserId);
        if (userData == null)
        {
            userData = new Models.UserData();
            await _context.AddAsync(userData);
        }

        Map(data, userData);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var data = _context.UsersData.FirstOrDefault(data => data.UserId == id);
        if (data == null) return;
        _context.Remove(data);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserData>> GetUsersWithExpiredSubscribesAsync()
    {
        var data = await _context.UsersData.Where(d => d.Subscribes.Any(s => s.EndSubscribe < DateTime.UtcNow))
            .ToListAsync();
        return data.Select(Map).ToList();
    }
}