using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Specification;
using LikeBotVK.Domain.VK.Specification;

namespace LikeBotVK.Application.Services.Services;

public class UserJobService : IUserJobService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserJobService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Job>> GetUserNotStartedJobs(long userId)
    {
        var vks = await _unitOfWork.VkRepository.Value.FindAsync(new UserVksSpecification(userId));
        var list = new List<Job>();
        foreach (var vk in vks)
            list.AddRange(await _unitOfWork.JobRepository.Value.FindAsync(new VkNotStartedJobsSpecification(vk.Id)));
        return list;
    }
}