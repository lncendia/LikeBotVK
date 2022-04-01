using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Application.Services.Services;

public class RandomProxySetter : IProxySetter
{
    private readonly IUnitOfWork _unitOfWork;

    public RandomProxySetter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SetProxyAsync(Vk vk)
    {
        var count = await _unitOfWork.ProxyRepository.Value.CountAsync(null);
        if (count == 0) return false;
        var random = new Random().Next(count);
        var randomProxy = (await _unitOfWork.ProxyRepository.Value.FindAsync(null, random, 1)).First();
        vk.SetProxy(randomProxy);
        return true;
    }
}