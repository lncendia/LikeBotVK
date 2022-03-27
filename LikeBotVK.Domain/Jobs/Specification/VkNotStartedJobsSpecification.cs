using Ardalis.Specification;

namespace LikeBotVK.Domain.Jobs.Specification;

public sealed class VkNotStartedJobsSpecification : VkJobsSpecification
{
    public VkNotStartedJobsSpecification(int id) : base(id)
    {
        Query.Where(work => work.StartTime == default);
    }
}