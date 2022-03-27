using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Configuration;

public class DatabaseConfiguration
{
    [Required] public string BaseConnection { get; init; } = null!;
    [Required] public string DomainDb { get; init; } = null!;
    [Required] public string ApplicationDb { get; init; } = null!;
    [Required] public string HangfireDb { get; init; } = null!;
}