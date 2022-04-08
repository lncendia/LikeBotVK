using LikeBotVK.Application.Abstractions.ApplicationData;

namespace LikeBotVK.Application.Abstractions.Services.BotServices;

public interface IJobScheduler
{
    public Task StartWorkAsync(JobData job);
    public Task ScheduleWorkAsync(JobData job, DateTimeOffset start);
    public Task<bool> CancelWorkAsync(JobData job);
}