using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Domain.Jobs.Specification;

public class JobsFromIdsSpecification : ISpecification<Job, IJobSpecificationVisitor>
{
    public ICollection<int> Ids { get; }
    public JobsFromIdsSpecification(ICollection<int> ids) => Ids = ids;

    public bool IsSatisfiedBy(Job item) => Ids.Contains(item.Id);

    public void Accept(IJobSpecificationVisitor visitor) => visitor.Visit(this);
}