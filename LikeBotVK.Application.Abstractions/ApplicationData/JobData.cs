using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Application.Abstractions.ApplicationData;

public class JobData
{
    public JobData(int jobId) => JobId = jobId;
    public int JobId { get; }
    public string? BackgroundJobId { get; set; }
    public string? Hashtag { get; set; }
    public WorkType? WorkType { get; set; }
    public DateTime? DateTimeLimitation { get; set; }
}