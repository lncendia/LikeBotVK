using LikeBotVK.Domain.Abstractions.Factories;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.VK.Entities;
using LikeBotVK.Domain.VK.Exceptions;
using LikeBotVK.Domain.VK.Specification;

namespace LikeBotVK.Domain.Services.Factories;

public class VkFactory : IVkFactory
{
    private readonly IUnitOfWork _unitOfWork;

    public VkFactory(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Vk> CreateVkAsync(long userId, string username, string password)
    {
        var user = await _unitOfWork.UserRepository.Value.GetAsync(userId);
        if (user == null) throw new ArgumentException("Couldn't find a user with this ID", nameof(userId));
        var vks = await _unitOfWork.VkRepository.Value.FindAsync(new UserVksSpecification(userId));
        if (vks.Count >= user.Subscribes.Count) throw new NoSubscriptionException();
        return new Vk(userId, username, password);
    }
}