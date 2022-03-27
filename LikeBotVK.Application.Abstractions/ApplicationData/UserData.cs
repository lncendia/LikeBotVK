using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Application.Abstractions.ApplicationData;

public class UserData
{
    public long UserId { get; set; }
    public State State { get; set; }
    public int? CurrentVkId { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }

    public long? ReferralId { get; set; }
    public decimal BonusAccount { get; set; }
}