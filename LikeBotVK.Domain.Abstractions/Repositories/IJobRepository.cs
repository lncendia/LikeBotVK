using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification.Visitor;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IJobRepository : IRepository<Job, int, IJobSpecificationVisitor>
{
}