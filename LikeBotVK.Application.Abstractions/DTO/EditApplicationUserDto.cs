using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Application.Abstractions.DTO;

public class EditApplicationUserDto
{
    public long Id { get; set; }
    public State State { get; set; }
    public decimal BonusAccount { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsBanned { get; set; }
}