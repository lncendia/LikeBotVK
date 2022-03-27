using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.Jobs.Entities;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IJobRepository : IRepository<Job, int>
{
}