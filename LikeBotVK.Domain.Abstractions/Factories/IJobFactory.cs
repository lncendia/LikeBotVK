using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Abstractions.Factories;

public interface IJobFactory
{
    Task<Job> CreateJobAsync(int vkId);
}