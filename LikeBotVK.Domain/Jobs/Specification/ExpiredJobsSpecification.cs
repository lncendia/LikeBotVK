using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;

namespace LikeBotVK.Domain.Jobs.Specification;

public class ExpiredJobsSpecification : ISpecification<Job, IJobSpecificationVisitor>
{
    public DateTime ExpiredTime { get; }

    public ExpiredJobsSpecification(DateTime expiredTime)
    {
        ExpiredTime = expiredTime;
    }

    public bool IsSatisfiedBy(Job item)
    {
        return item.StartTime.HasValue && item.StartTime < ExpiredTime;
    }

    public void Accept(IJobSpecificationVisitor visitor) => visitor.Visit(this);
}