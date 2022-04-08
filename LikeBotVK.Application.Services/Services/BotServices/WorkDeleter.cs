using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Jobs.Specification;

namespace LikeBotVK.Application.Services.Services.BotServices;

public class WorkDeleter : IWorkDeleter
{
    private readonly IUnitOfWork _unitOfWork;

    public WorkDeleter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task DeleteAsync()
    {
        var works = await _unitOfWork.JobRepository.Value.FindAsync(
            new ExpiredJobsSpecification(DateTime.UtcNow.AddDays(-15)));
        await _unitOfWork.JobRepository.Value.DeleteRangeAsync(works);
    }
}