using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Specification.Visitor;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IVkRepository : IRepository<Vk, int, IVkSpecificationVisitor>
{
}