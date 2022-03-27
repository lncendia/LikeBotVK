using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Configuration;

public class Configuration
{
    [Required] public BotConfiguration BotConfiguration { get; init; } = null!;
    [Required] public DatabaseConfiguration DatabaseConfiguration { get; init; } = null!;
    [Required] public PaymentConfiguration PaymentConfiguration { get; init; } = null!;
    [Required] public string AntiCaptchaToken { get; init; } = null!;
}