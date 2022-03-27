using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.Payments.Entities;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IVkRepository : IRepository<Vk, int>
{
}