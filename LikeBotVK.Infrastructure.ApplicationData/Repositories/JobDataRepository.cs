using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData.Context;
using Microsoft.EntityFrameworkCore;

namespace LikeBotVK.Infrastructure.ApplicationData.Repositories;

public class JobDataRepository : IJobDataRepository
{
    private readonly ApplicationDbContext _context;

    public JobDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<JobData?> GetAsync(int id)
    {
        return _context.JobsData.Where(data => data.JobId == id).Select(data => new JobData(data.JobId)
        {
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
            jobData = new Models.JobData();
            await _context.AddAsync(jobData);
        }

        jobData.JobId = data.JobId;
        jobData.BackgroundJobId = data.BackgroundJobId;
        jobData.DateTimeLimitation = data.DateTimeLimitation;
        jobData.Hashtag = data.Hashtag;
        jobData.WorkType = data.WorkType;

        await _context.SaveChangesAsync();
    }

    public Task<List<JobData>> FindAsync(int? skip, int? take)
    {
        var query = _context.JobsData.AsQueryable();
        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);
        return query.Select(data => new JobData(data.JobId)
        {
            BackgroundJobId = data.BackgroundJobId,
            DateTimeLimitation = data.DateTimeLimitation,
            Hashtag = data.Hashtag,
            WorkType = data.WorkType
        }).ToListAsync();
    }

    public Task<int> CountAsync() => _context.JobsData.CountAsync();

    public async Task DeleteAsync(JobData data)
    {
        _context.Remove(_context.JobsData.First(data1 => data1.JobId == data.JobId));
        await _context.SaveChangesAsync();
    }
}