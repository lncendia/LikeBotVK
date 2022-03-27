using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData.Context;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.ApplicationData.Services;

public class JobDataRepository : IJobDataRepository
{
    private readonly ApplicationDbContext _context;

    public JobDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<JobData?> GetAsync(int id)
    {
        return _context.JobsData.Where(data => data.JobId == id).Select(data => new JobData
        {
            JobId = data.JobId,
            BackgroundJobId = data.BackgroundJobId,
            DateTimeLimitation = data.DateTimeLimitation,
            Hashtag = data.Hashtag,
            WorkType = data.WorkType
        }).FirstOrDefaultAsync();
    }

    public async Task AddOrUpdateAsync(JobData data)
    {
        var jobData = _context.JobsData.FirstOrDefault(data1 => data1.JobId == data.JobId);
        if (jobData == null)
        {
            await _context.AddAsync(new Models.JobData()
            {
                JobId = data.JobId,
                BackgroundJobId = data.BackgroundJobId,
                DateTimeLimitation = data.DateTimeLimitation,
                Hashtag = data.Hashtag,
                WorkType = data.WorkType
            });
        }
        else
        {
            jobData.JobId = data.JobId;
            jobData.BackgroundJobId = data.BackgroundJobId;
            jobData.DateTimeLimitation = data.DateTimeLimitation;
            jobData.Hashtag = data.Hashtag;
            jobData.WorkType = data.WorkType;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var data = _context.JobsData.FirstOrDefault(data => data.JobId == id);
        if (data == null) return;
        _context.Remove(data);
        await _context.SaveChangesAsync();
    }
}