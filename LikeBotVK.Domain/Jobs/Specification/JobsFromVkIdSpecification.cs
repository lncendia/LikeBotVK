using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;

namespace LikeBotVK.Domain.Jobs.Specification;

public class JobsFromVkIdSpecification : ISpecification<Job, IJobSpecificationVisitor>
{
    public int Id { get; }
    public JobsFromVkIdSpecification(int id) => Id = id;

    public bool IsSatisfiedBy(Job item) => item.VkId == Id;

    public void Accept(IJobSpecificationVisitor visitor) => visitor.Visit(this);
}