using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Domain.Abstractions.Factories;

public interface IVkFactory
{
    Task<Vk> CreateVkAsync(long userId, string username, string password);
}