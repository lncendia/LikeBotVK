using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Jobs.Exceptions;

public sealed class JobVkNotFoundException : Exception
{
    public JobVkNotFoundException(Job job) : base($"Vk of job {job.Id} is not found.")
    {
    }
}