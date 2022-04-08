using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Configuration;

public class PaymentConfiguration
{
    [Required] public string QiwiToken { get; init; } = null!;
    [Required] public decimal SubscribeCost { get; init; }

    [Required] public decimal ReferralBonus { get; init; }

    [Required] public int SubscribeDuration { get; init; }
}