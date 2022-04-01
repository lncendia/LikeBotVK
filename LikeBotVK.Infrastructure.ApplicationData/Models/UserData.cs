using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Infrastructure.ApplicationData.Models;

public class UserData
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long UserId { get; set; }

    public State State { get; set; }
    public bool IsBanned { get; set; }
    public bool IsAdmin { get; set; }
    public int? CurrentVkId { get; set; }
    public List<int> CurrentJobsId { get; set; } = new();
    public List<SubscribeData> Subscribes { get; set; } = new();

    public long? ReferralId { get; set; }
    public decimal BonusAccount { get; set; }
}