using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Application.Services.BotCommands;

public static class JobDivider
{
    public static async Task StartDivideJobs(Job job, JobData data, ServiceFacade facade, DateTime startTimeUtc)
    {
        for (var i = 1; i < 4; i++)
        {
            var newJob = new Job(job.VkId)
                {Type = job.Type};
            newJob.SetInterval(job.LowerInterval, job.UpperInterval);
            await facade.UnitOfWork.JobRepository.Value.AddAsync(newJob);
            var date = startTimeUtc.AddHours(6 * i);
            var newData = new JobData(newJob.Id)
            {
                Hashtag = data.Hashtag,
                DateTimeLimitation = date,
                WorkType = WorkType.Simple
            };
            try
            {
                await facade.JobScheduler.ScheduleWorkAsync(newData, new DateTimeOffset(date));
                await facade.ApplicationDataUnitOfWork.JobDataRepository.Value.AddOrUpdateAsync(newData);
            }
            catch (ErrorStartJobException)
            {
                await facade.UnitOfWork.JobRepository.Value.DeleteAsync(newJob);
            }
        }
    }
}