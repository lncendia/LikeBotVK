using System.Collections.ObjectModel;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Exceptions;
using VkNet.Abstractions;
using VkNet.Model;
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
        _token = token;
    }

    public async Task<List<Publication>> GetPublicationsAsync(Vk vk, string hashtag, Type type, int count,
        DateTime? limitTime, CancellationToken token)
    {
        if (string.IsNullOrEmpty(vk.AccessToken)) throw new VkNotActiveException(vk);
        if (count < 1) throw new ArgumentException("Count can't be less then zero.");
        if (string.IsNullOrEmpty(hashtag)) throw new ArgumentException("Hashtag can't be null or empty.");

        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        var publications = await GetNewsAsync(VkApi.BuildApi(vk.AccessToken, _token, proxy), hashtag,
            limitTime?.ToLocalTime(), count, token);
        if (publications?.Items == null || !publications.Items.Any())
            throw new PublicationsNotFoundException(hashtag);
        var items = type switch
        {
            Type.Like => publications.Items.Where(item => item.Likes.CanLike && !item.Likes.UserLikes),
            Type.Subscribe => publications.Items,
            Type.Repost => publications.Items.Where(item => item.Likes.CanPublish!.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        var list = items.Select(item => new Publication {OwnerId = item.FromId, PublicationId = item.Id}).ToList();
        if (!list.Any()) throw new PublicationsNotFoundException(hashtag);
        return list;
    }


    private static async Task<NewsSearchResult?> GetNewsAsync(IVkApiCategories api, string query,
        DateTime? startTimeLocal, int count, CancellationToken token)
    {
        var pages = count / 200;
        var result = new NewsSearchResult();
        for (var i = 0; i < pages; i++)
        {
            await Task.Delay(3000, token);
            var response = await api.NewsFeed.SearchAsync(new NewsFeedSearchParams
                {StartTime = startTimeLocal, Query = query, Count = 200, StartFrom = result.NextFrom});
            if (!response.Items.Any()) return result;
            Map(response, result);
            if (string.IsNullOrEmpty(result.NextFrom)) return result;
        }

        var rest = count % 200;
        if (rest < 1) return result;
        {
            token.ThrowIfCancellationRequested();
            var response = await api.NewsFeed
                .SearchAsync(new NewsFeedSearchParams
                    {StartTime = startTimeLocal, Query = query, Count = rest, StartFrom = result.NextFrom});
            if (!response.Items.Any()) return result;
            Map(response, result);
        }

        return result;
    }

    private static void Map(NewsSearchResult source, NewsSearchResult destination)
    {
        destination.Count = source.Count;
        var items = destination.Items?.ToList() ?? new List<NewsSearchItem>();
        items.AddRange(source.Items);
        destination.Items = new ReadOnlyCollection<NewsSearchItem>(items);
        destination.NextFrom = source.NextFrom;
        source.TotalCount = source.TotalCount;
    }
}