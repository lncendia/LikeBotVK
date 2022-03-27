using Ardalis.Specification;

namespace LikeBotVK.Domain.Jobs.Specification;

public sealed class VkFinishedJobsSpecification : VkJobsSpecification
{
    public VkFinishedJobsSpecification(int id) : base(id)
    {
        Query.Where(work => work.IsCompleted);
    }
}