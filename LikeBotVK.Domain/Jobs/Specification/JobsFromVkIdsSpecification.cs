using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications;

namespace LikeBotVK.Domain.Jobs.Specification;

public class JobsFromVkIdsSpecification : ISpecification<Job, IJobSpecificationVisitor>
{
    public ICollection<int> VkIds { get; }
    public JobsFromVkIdsSpecification(ICollection<int> vkIds) => VkIds = vkIds;

    public bool IsSatisfiedBy(Job item) => VkIds.Contains(item.VkId);

    public void Accept(IJobSpecificationVisitor visitor) => visitor.Visit(this);
}