using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Application.Abstractions.Services.BotServices;

public interface IProxySetter
{
    Task<bool> SetProxyAsync(Vk vk);
}