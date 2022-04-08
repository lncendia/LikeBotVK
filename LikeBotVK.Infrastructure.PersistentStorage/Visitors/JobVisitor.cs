using System.Linq.Expressions;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.Jobs.Specification.Visitor;
using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Infrastructure.PersistentStorage.Models;

namespace LikeBotVK.Infrastructure.PersistentStorage.Visitors;

public class JobVisitor : BaseVisitor<JobModel, IJobSpecificationVisitor, Job>, IJobSpecificationVisitor
{
    public void Visit(ExpiredJobsSpecification specification) =>
        Expr = Expr = j => j.StartTime < specification.ExpiredTime;

    public void Visit(FinishedJobsSpecification specification) => Expr = j => j.IsCompleted;

    public void Visit(JobsFromIdsSpecification specification) =>
        Expr = j => specification.Ids.Contains(j.Id);

    public void Visit(JobsFromVkIdSpecification specification) =>
        Expr = j => j.VkId == specification.Id;

    public void Visit(JobsFromVkIdsSpecification specification) =>
        Expr = j => specification.VkIds.Contains(j.VkId);

    protected override Expression<Func<JobModel, bool>> ConvertSpecToExpression(
        ISpecification<Job, IJobSpecificationVisitor> spec)
    {
        var visitor = new JobVisitor();
        spec.Accept(visitor);
        return visitor.Expr!;
    }
}