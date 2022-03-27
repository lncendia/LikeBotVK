namespace LikeBotVK.Application.Abstractions.Configuration;

public class Configuration
{
    public Configuration(string helpAddress, string instructionAddress, decimal subscribeCost, decimal referralBonus,
        int subscribeDuration, List<Project> projects)
    {
        HelpAddress = helpAddress;
        InstructionAddress = instructionAddress;
        SubscribeCost = subscribeCost;
        ReferralBonus = referralBonus;
        SubscribeDuration = subscribeDuration;
        Projects = projects;
    }

    public string HelpAddress { get; }
    public string InstructionAddress { get; }
    public decimal SubscribeCost { get; }
    public decimal ReferralBonus { get; }
    public int SubscribeDuration { get; }
    public List<Project> Projects { get; }
}

public class Project
{
    public Project(string link, string name)
    {
        Link = link;
        Name = name;
    }

    public string Link { get; }
    public string Name { get; }
}