using LikeBotVK.Domain.Jobs.ValueObjects;

namespace LikeBotVK.Domain.Jobs.Entities;

public class Job
{
    public Job(int vkId)
    {
        VkId = vkId;
    }

    public int Id { get; set; }
    public int VkId { get; set; }
    public List<Publication> Publications { get; set; } = new();
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public LikeBotVK.Domain.Jobs.Enums.Type Type { get; set; }
    public DateTime? StartTime { get; set; }
    public bool IsCompleted { get; set; }

    public string? ErrorMessage { get; set; }
    public int CountErrors { get; set; }
    public int CountSuccess { get; set; }

    public void SetPublications(List<Publication> publications) => Publications = publications;

    public void SetInterval(int lower, int upper)
    {
        LowerInterval = lower;
        UpperInterval = upper;
    }

    public void CompleteJob() => IsCompleted = true;

    private readonly Random _random = new();

    public Task Delay(CancellationToken token) => Task.Delay(_random.Next(LowerInterval, UpperInterval), token);
}