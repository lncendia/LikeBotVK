using Ardalis.Specification;

namespace LikeBotVK.Domain.Jobs.Specification;

public sealed class VkNotFinishedJobsSpecification : VkJobsSpecification
{
    public VkNotFinishedJobsSpecification(int id) : base(id)
    {
        Query.Where(job => job.IsCompleted);
    }
}