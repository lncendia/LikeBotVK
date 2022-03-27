using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.Repositories;

public interface IJobDataRepository
{
    Task<JobData?> GetAsync(int id);
    Task AddOrUpdateAsync(JobData data);
    Task DeleteAsync(int id);
}