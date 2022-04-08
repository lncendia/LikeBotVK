using LikeBotVK.Domain.Jobs.ValueObjects;

namespace LikeBotVK.Domain.Jobs.Entities;

public class Job
{
    public Job(int vkId)
    {
        VkId = vkId;
    }

    public int Id { get; set; }
    public int VkId { get; }

    private readonly List<Publication> _publications = new();

    public List<Publication> Publications => _publications.ToList();

    public int UpperInterval { get; private set; }
    public int LowerInterval { get; private set; }
    public LikeBotVK.Domain.Jobs.Enums.Type Type { get; set; }
    public DateTime? StartTime { get; private set; }
    public bool IsCompleted { get; private set; }

    public string? ErrorMessage { get; set; }
    public int CountErrors { get; private set; }
    public int CountSuccess { get; private set; }

    public void SetInterval(int lower, int upper)
    {
        if (lower > upper) throw new ArgumentException("Lower delay can't be bigger then upper.", nameof(lower));
        LowerInterval = lower;
        UpperInterval = upper;
    }

    public void UpdateInfo(int errors, int success)
    {
        if (errors + success > _publications.Count)
            throw new ArgumentException("Count errors and success can't be bigger then publications count.",
                nameof(errors));
        if (errors < CountErrors || success < CountSuccess)
            throw new ArgumentException("The information cannot be reduced.");
        CountErrors = errors;
        CountSuccess = success;
    }

    public void MarkAsStarted() => StartTime = DateTime.UtcNow;

    public void MarkAsCompleted()
    {
        if (!StartTime.HasValue) MarkAsStarted();
        IsCompleted = true;
    }

    public void AddPublications(IEnumerable<Publication> publications) => _publications.AddRange(publications);

    private readonly Random _random = new();

    public Task Delay(CancellationToken token) => Task.Delay(_random.Next(LowerInterval, UpperInterval) * 1000, token);
}