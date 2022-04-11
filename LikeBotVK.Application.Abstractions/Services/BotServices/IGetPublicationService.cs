using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Domain.VK.Entities;
using Type = LikeBotVK.Domain.Jobs.Enums.Type;

namespace LikeBotVK.Application.Abstractions.Services.BotServices;

public interface IGetPublicationService
{
    Task<List<Publication>> GetPublicationsAsync(Vk vk, string hashtag, Type type, int count, DateTime? limitTime,
        CancellationToken token);
}