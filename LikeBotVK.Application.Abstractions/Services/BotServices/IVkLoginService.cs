using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Application.Abstractions.Services.BotServices;

public interface IVkLoginService
{
    Task<LoginResult> ActivateAsync(Vk vk);
    Task DeactivateAsync(Vk vk);
    Task<LoginResult> EnterTwoFactorAsync(Vk vk, string code);
}