using Ardalis.Specification;
using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Jobs.Specification;

public class VkJobsSpecification : Specification<Job>
{
    public VkJobsSpecification(int id)
    {
        Query.Where(work => work.VkId == id);
    }
}