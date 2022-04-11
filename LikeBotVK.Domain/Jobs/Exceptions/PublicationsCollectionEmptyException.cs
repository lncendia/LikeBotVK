using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Jobs.Exceptions;

public sealed class PublicationsCollectionEmptyException : Exception
{
    public PublicationsCollectionEmptyException(Job job) : base($"Publications collection of job {job.Id} is empty.")
    {
    }
}