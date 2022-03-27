using System.Net;
using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Infrastructure.PublicationsGetter.AntiCaptcha;
using Microsoft.Extensions.DependencyInjection;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using VkNet.Utils.AntiCaptcha;

namespace LikeBotVK.Infrastructure.PublicationsGetter;

public static class VkApi
{
    public static VkNet.VkApi BuildApi(string accessToken, string antiCaptcha, Proxy? proxy)
    {
        var services = new ServiceCollection();
        services.AddScoped<ICaptchaSolver, CaptchaSolver>(_ => new CaptchaSolver(antiCaptcha));
        services.AddAudioBypass();
        if (proxy != null)
            services.AddSingleton(_ => GetHttpClientWithProxy(proxy));

        var api = new VkNet.VkApi(services);
        api.Authorize(new ApiAuthParams {AccessToken = accessToken});
        return api;
    }

    private static HttpClient GetHttpClientWithProxy(Proxy proxy)
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
    }
}