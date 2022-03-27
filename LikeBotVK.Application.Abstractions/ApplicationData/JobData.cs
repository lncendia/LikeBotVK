using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Application.Abstractions.ApplicationData;

public class JobData
{
    public int JobId { get; set; }
    public string? BackgroundJobId { get; set; }
    public string? Hashtag { get; set; }
    public WorkType? WorkType { get; set; }
    public DateTime? DateTimeLimitation { get; set; }
}