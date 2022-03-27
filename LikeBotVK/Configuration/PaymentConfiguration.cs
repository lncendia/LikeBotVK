using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Configuration;

public class PaymentConfiguration
{
    [Required] public string QiwiToken { get; init; } = null!;
    [Required] public decimal SubscribeCost => 0;

    [Required] public decimal ReferralBonus => 0;

    [Required] public int SubscribeDuration => 0;
}