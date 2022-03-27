using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Exceptions;
using VkNet.Model.RequestParams;
using Type = LikeBotVK.Domain.Jobs.Enums.Type;

namespace LikeBotVK.Infrastructure.PublicationsGetter.Services;

public class GetPublicationService : IGetPublicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _token;

    public GetPublicationService(IUnitOfWork unitOfWork, string token)
    {
        _unitOfWork = unitOfWork;
        this._token = token;
    }

    public async Task<List<Publication>> GetPublicationsAsync(Vk vk, string hashtag, Type type, DateTime? limitTime,
        CancellationToken token)
    {
        if (string.IsNullOrEmpty(vk.AccessToken)) throw new VkNotActiveException(vk);
        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        var publications = await VkApi.BuildApi(vk.AccessToken, _token, proxy).NewsFeed
            .SearchAsync(new NewsFeedSearchParams {EndTime = limitTime, Query = hashtag});
        var items = type switch
        {
            Type.Like => publications.Items.Where(item => item.Likes.CanLike && !item.Likes.UserLikes),
            Type.Subscribe => publications.Items,
            Type.Repost => publications.Items.Where(item => item.Likes.CanPublish!.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        return items.Select(item => new Publication {OwnerId = item.FromId, PublicationId = item.Id}).ToList();
    }
}