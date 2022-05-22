using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.Specification;

namespace LikeBotVK.Application.Services.Services.BotServices;

public class WorkDeleter : IWorkDeleter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDataUnitOfWork _applicationDataUnitOfWork;

    public WorkDeleter(IUnitOfWork unitOfWork, IApplicationDataUnitOfWork applicationDataUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _applicationDataUnitOfWork = applicationDataUnitOfWork;
    }

    public async Task DeleteAsync()
    {
        var works = await _unitOfWork.JobRepository.Value.FindAsync(
            new ExpiredJobsSpecification(DateTime.UtcNow.AddDays(-15)));
        foreach (var job in works)
        {
            var jobData = await _applicationDataUnitOfWork.JobDataRepository.Value.GetAsync(job.Id);
            if (jobData != null)
                await _applicationDataUnitOfWork.JobDataRepository.Value.DeleteAsync(jobData);
        }

        await _unitOfWork.JobRepository.Value.DeleteRangeAsync(works);
    }
}