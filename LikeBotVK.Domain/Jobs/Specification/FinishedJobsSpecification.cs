using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;

namespace LikeBotVK.Domain.Jobs.Specification;

public class FinishedJobsSpecification : ISpecification<Job, IJobSpecificationVisitor>
{
    public bool IsSatisfiedBy(Job item) => item.IsCompleted;
    public void Accept(IJobSpecificationVisitor visitor) => visitor.Visit(this);
}