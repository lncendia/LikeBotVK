using Ardalis.Specification;
using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Jobs.Specification;

public sealed class ExpiredJobsSpecification : Specification<Job>
{
    public ExpiredJobsSpecification(DateTime dateLimitation)
    {
        Query.Where(work => work.IsCompleted && DateTime.UtcNow < dateLimitation);
    }
}