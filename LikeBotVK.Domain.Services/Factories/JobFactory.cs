using LikeBotVK.Domain.Abstractions.Factories;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.VK.Exceptions;
using NoSubscriptionException = LikeBotVK.Domain.Jobs.Exceptions.NoSubscriptionException;

namespace LikeBotVK.Domain.Services.Factories;

public class JobFactory : IJobFactory
{
    private readonly IUnitOfWork _unitOfWork;

    public JobFactory(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Job> CreateJobAsync(int vkId)
    {
        var vk = await _unitOfWork.VkRepository.Value.GetAsync(vkId);
        if (vk == null) throw new ArgumentException("Couldn't find a VK with this ID", nameof(vkId));
        if (string.IsNullOrEmpty(vk.AccessToken)) throw new VkNotActiveException(vk);
        var user = await _unitOfWork.UserRepository.Value.GetAsync(vk.UserId);
        var notStarted = await _unitOfWork.JobRepository.Value.CountAsync(new VkNotStartedJobsSpecification(vkId));
        if (user!.Subscribes.Count >= notStarted) throw new NoSubscriptionException();
        return new Job(vkId);
    }
}