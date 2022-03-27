using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Configuration;

public class BotConfiguration
{
    [Required] public string TelegramToken { get; set; } = null!;
    [Required] public string HelpAddress { get; set; } = null!;
    [Required] public string InstructionAddress { get; set; } = null!;
    [Required] public List<Project> Projects { get; set; } = null!;
}

public class Project
{
    [Required] public string Link { get; set; } = null!;
    [Required] public string Name { get; set; } = null!;
}