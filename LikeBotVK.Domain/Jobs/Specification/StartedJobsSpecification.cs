using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Domain.Jobs.Specification;

public class StartedJobsSpecification : ISpecification<Job, IJobSpecificationVisitor>
{
    public bool IsSatisfiedBy(Job item)
    {
        return item.StartTime.HasValue;
    }

    public void Accept(IJobSpecificationVisitor visitor) => visitor.Visit(this);
}