using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Application.Abstractions.DTO;

public class UserDto
{
    public UserDto(long id, bool isAdmin, bool isBanned)
    {
        Id = id;
        IsAdmin = isAdmin;
        IsBanned = isBanned;
    }

    public long Id { get; }
    public bool IsAdmin { get; }
    public bool IsBanned { get; }
}

public class UserExtendedDto : UserDto
{
    public UserExtendedDto(long id, bool isAdmin, bool isBanned, decimal bonusAccount, State state) : base(id, isAdmin,
        isBanned)
    {
        BonusAccount = bonusAccount;
        State = state;
    }

    public State State { get; }
    public decimal BonusAccount { get; }
    public List<DateTime> Subscribes { get; } = new();

    public void AddSubscribes(IEnumerable<DateTime> dateTimes)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        Subscribes.AddRange(dateTimes.Select(x => TimeZoneInfo.ConvertTimeFromUtc(x, tz)));
    }
}