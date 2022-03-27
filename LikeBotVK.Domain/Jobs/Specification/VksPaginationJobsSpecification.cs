using Ardalis.Specification;
using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Jobs.Specification;

public sealed class VksPaginationJobsSpecification : Specification<Job>
{
    public VksPaginationJobsSpecification(List<int> vkIds, int skip, int take)
    {
        Query.Where(job => vkIds.Contains(job.VkId)).Skip(skip).Take(take);
    }
}