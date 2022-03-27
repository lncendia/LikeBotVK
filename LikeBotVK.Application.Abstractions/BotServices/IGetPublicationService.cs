using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Domain.VK.Entities;
using Type = LikeBotVK.Domain.Jobs.Enums.Type;

namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IGetPublicationService
{
     Task<List<Publication>> GetPublicationsAsync(Vk vk, string hashtag, Type type, DateTime? limitTime,
          CancellationToken token);
}