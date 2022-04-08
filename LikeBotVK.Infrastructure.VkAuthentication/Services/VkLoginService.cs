using LikeBotVK.Application.Abstractions.Enums;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Infrastructure.VkAuthentication.Exceptions;
using VkNet.AudioBypassService.Exceptions;

namespace LikeBotVK.Infrastructure.VkAuthentication.Services;

public class VkLoginService : IVkLoginService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly VkApi _api;

    public VkLoginService(IUnitOfWork unitOfWork, string antiCaptchaToken)
    {
        _unitOfWork = unitOfWork;
        _api = new VkApi(antiCaptchaToken);
    }

    public async Task<LoginResult> ActivateAsync(Vk vk)
    {
        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        try
        {
            await _api.Activate(vk, proxy);
            return LoginResult.Succeeded;
        }
        catch (VkAuthException)
        {
            return LoginResult.BadData;
        }
        catch (TwoFactorRequiredException)
        {
            return LoginResult.TwoFactorRequired;
        }
    }

    public async Task DeactivateAsync(Vk vk)
    {
        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        if (!string.IsNullOrEmpty(vk.AccessToken))
            await _api.BuildApi(vk.AccessToken, proxy).LogOutAsync();
    }

    public async Task<LoginResult> EnterTwoFactorAsync(Vk vk, string code)
    {
        var proxy = vk.ProxyId.HasValue ? await _unitOfWork.ProxyRepository.Value.GetAsync(vk.ProxyId.Value) : null;
        try
        {
            await _api.ActivateWithTwoFactor(vk, proxy, code);
            return LoginResult.Succeeded;
        }
        catch (VkAuthException)
        {
            return LoginResult.BadData;
        }
        catch (TwoFactorRequiredException)
        {
            return LoginResult.TwoFactorRequired;
        }
    }
}