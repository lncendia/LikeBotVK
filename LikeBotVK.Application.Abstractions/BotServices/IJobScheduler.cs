using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IJobScheduler
{
    public Task StartWorkAsync(JobData job);
    public Task ScheduleWorkAsync(JobData job, DateTimeOffset start);
    public Task<bool> CancelWorkAsync(JobData job);
}