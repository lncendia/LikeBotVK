using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Specifications.Abstractions;

namespace LikeBotVK.Domain.Jobs.Specification.Visitor;

public interface IJobSpecificationVisitor : ISpecificationVisitor<IJobSpecificationVisitor, Job>
{
    void Visit(ExpiredJobsSpecification specification);
    void Visit(FinishedJobsSpecification specification);
    void Visit(JobsFromIdsSpecification specification);
    void Visit(JobsFromVkIdSpecification specification);
    void Visit(JobsFromVkIdsSpecification specification);
}