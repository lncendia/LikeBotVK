using System.Net;
using LikeBotVK.Domain.Proxies.Entities;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Infrastructure.VkAuthentication.AntiCaptcha;
using LikeBotVK.Infrastructure.VkAuthentication.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using VkNet.AudioBypassService.Extensions;
using VkNet.Model;
using VkNet.Utils.AntiCaptcha;

namespace LikeBotVK.Infrastructure.VkAuthentication;

public class VkApi
{
    private readonly string _antiCaptchaToken;

    public VkApi(string antiCaptchaToken)
    {
        _antiCaptchaToken = antiCaptchaToken;
    }

    public VkNet.VkApi BuildApi(string accessToken, Proxy? proxy)
    {
        var services = new ServiceCollection();
        services.AddAudioBypass();
        services.AddScoped<ICaptchaSolver, CaptchaSolver>(_ => new CaptchaSolver(_antiCaptchaToken));
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

    public async Task Activate(Vk vk, Proxy? proxy)
    {
        var services = new ServiceCollection();
        services.AddAudioBypass();
        services.AddScoped<ICaptchaSolver, CaptchaSolver>(_ => new CaptchaSolver(_antiCaptchaToken));
        if (proxy != null)
            services.AddSingleton(_ => GetHttpClientWithProxy(proxy));
        var api = new VkNet.VkApi(services);

        await api.AuthorizeAsync(new ApiAuthParams
        {
            Login = vk.Username,
            Password = vk.Password,
            TwoFactorAuthorization = () => throw new TwoFactorRequiredException()
        });
        vk.AccessToken = api.Token;
    }

    public async Task ActivateWithTwoFactor(Vk vk, Proxy? proxy, string code)
    {
        var services = new ServiceCollection();
        services.AddAudioBypass();
        services.AddScoped<ICaptchaSolver, CaptchaSolver>(_ => new CaptchaSolver(_antiCaptchaToken));
        if (proxy != null)
            services.AddSingleton(_ => GetHttpClientWithProxy(proxy));
        var api = new VkNet.VkApi(services);

        await api.AuthorizeAsync(new ApiAuthParams
        {
            Login = vk.Username,
            Password = vk.Password,
            TwoFactorAuthorization = () => code
        });
        vk.AccessToken = api.Token;
    }
}