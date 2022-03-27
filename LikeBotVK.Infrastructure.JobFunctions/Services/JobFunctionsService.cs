using System.Net;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Services;
using LikeBotVK.Domain.Jobs.ValueObjects;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Exceptions;
using LikeBotVK.Infrastructure.JobFunctions.AntiCaptcha;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils.AntiCaptcha;

namespace LikeBotVK.Infrastructure.JobFunctions.Services;

public class JobFunctionsService : IJobFunctionsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _antiCaptchaToken;

    public JobFunctionsService(IUnitOfWork unitOfWork, string antiCaptchaToken)
    {
        _unitOfWork = unitOfWork;
        _antiCaptchaToken = antiCaptchaToken;
    }

    public async Task LikeAsync(Vk vk, Publication publication)
    {
        if (string.IsNullOrEmpty(vk.AccessToken)) throw new VkNotActiveException(vk);
        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        await BuildApi(vk.AccessToken, _antiCaptchaToken, proxy).Likes.AddAsync(new LikesAddParams
            {ItemId = publication.PublicationId, OwnerId = publication.OwnerId, Type = LikeObjectType.Post});
    }

    public async Task RepostAsync(Vk vk, Publication publication)
    {
        if (string.IsNullOrEmpty(vk.AccessToken)) throw new VkNotActiveException(vk);
        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        var x = await BuildApi(vk.AccessToken, _antiCaptchaToken, proxy).Wall.RepostAsync(
            $"wall{publication.OwnerId}_{publication.PublicationId}",
            string.Empty,
            null, false);
    }

    public async Task FollowAsync(Vk vk, Publication publication)
    {
        if (string.IsNullOrEmpty(vk.AccessToken)) throw new VkNotActiveException(vk);
        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        var api = BuildApi(vk.AccessToken, _antiCaptchaToken, proxy);
        if (publication.OwnerId > 0)
        {
            await api.Friends.AddAsync(publication.OwnerId, string.Empty, null);
        }
        else
        {
            var result = await api.Groups.JoinAsync(publication.OwnerId);
            if (!result) throw new Exception();
        }
    }

    private static VkApi BuildApi(string accessToken, string captcha, Proxy? proxy)
    {
        var services = new ServiceCollection();
        services.AddAudioBypass();
        services.AddScoped<ICaptchaSolver, CaptchaSolver>(_ => new CaptchaSolver(captcha));
        var api = new VkApi(services);
        if (proxy != null)
        {
            services.AddSingleton(_ =>
            {
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(proxy.Host, proxy.Port)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(proxy.Login, proxy.Password)
                    }
                };
                return new HttpClient(httpClientHandler);
            });
        }

        api.Authorize(new ApiAuthParams {AccessToken = accessToken});
        return api;
    }
}