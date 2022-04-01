using LikeBotVK.Application.Abstractions.BotServices;
using JobData = LikeBotVK.Application.Abstractions.ApplicationData.JobData;

namespace LikeBotVK.Infrastructure.JobScheduler.Services;

public class JobScheduler : IJobScheduler
{
    private readonly IJobStarterService _jobStarterService;

    public JobScheduler(IJobStarterService jobStarterService)
    {
        _jobStarterService = jobStarterService;
    }

    public Task StartWorkAsync(JobData job)
    { 
        job.BackgroundJobId = Hangfire.BackgroundJob.Enqueue(() => StartJobAsync(job.JobId, CancellationToken.None));
        return Task.CompletedTask;
    }

    public Task ScheduleWorkAsync(JobData job, DateTimeOffset start)
    {
        job.BackgroundJobId = Hangfire.BackgroundJob.Schedule(() => StartJobAsync(job.JobId, CancellationToken.None), start);
        return Task.CompletedTask;
    }

    public Task<bool> CancelWorkAsync(JobData job)
    {
        return Task.FromResult(Hangfire.BackgroundJob.Delete(job.BackgroundJobId));
    }

    public async Task StartJobAsync(int id, CancellationToken token)
    {
        await _jobStarterService.StartJobAsync(id, token);
    }
}