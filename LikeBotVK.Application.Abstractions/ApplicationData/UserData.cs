using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Application.Abstractions.ApplicationData;

public class UserData
{
    public long UserId { get; set; }
    public State State { get; set; }
    public int? CurrentVkId { get; set; }
    public List<int> CurrentJobsId { get; set; } = new();
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
    public List<Subscribe> Subscribes { get; set; } = new();
    public long? ReferralId { get; set; }
    public decimal BonusAccount { get; set; }

    public int ActiveSubscribesCount() => Subscribes.Count(s => !s.IsExpired());
}

public class Subscribe
{
    public Subscribe(DateTime endSubscribe)
    {
        EndSubscribe = endSubscribe;
    }

    public DateTime EndSubscribe { get; }
    public bool IsExpired() => EndSubscribe < DateTime.UtcNow;
}