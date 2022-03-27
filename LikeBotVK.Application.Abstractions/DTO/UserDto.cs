using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Application.Abstractions.DTO;

public class UserDto
{
    public long Id { get; set; }
    public State State { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
    public decimal BonusAccount { get; set; }
    public long? ReferralId { get; set; }
    public List<DateTime> Subscribes { get; set; } = new();
}