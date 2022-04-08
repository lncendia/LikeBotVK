using LikeBotVK.Application.Abstractions.Services.BotServices;
using JobData = LikeBotVK.Application.Abstractions.ApplicationData.JobData;

namespace LikeBotVK.Infrastructure.JobScheduler.Services;

public class JobScheduler : IJobScheduler
{
    public Task StartWorkAsync(JobData job)
    {
        job.BackgroundJobId =
            Hangfire.BackgroundJob.Enqueue<IJobStarterService>(x => x.StartJobAsync(job.JobId, CancellationToken.None));
        return Task.CompletedTask;
    }

    public Task ScheduleWorkAsync(JobData job, DateTimeOffset start)
    {
        job.BackgroundJobId =
            Hangfire.BackgroundJob.Schedule<IJobStarterService>(x => x.StartJobAsync(job.JobId, CancellationToken.None), start);
        return Task.CompletedTask;
    }

    public Task<bool> CancelWorkAsync(JobData job)
    {
        return Task.FromResult(Hangfire.BackgroundJob.Delete(job.BackgroundJobId));
    }
}