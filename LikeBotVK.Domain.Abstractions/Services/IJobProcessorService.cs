using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Abstractions.Services;

public interface IJobProcessorService
{
    Task ProcessJobAsync(Job job, CancellationToken token);
}