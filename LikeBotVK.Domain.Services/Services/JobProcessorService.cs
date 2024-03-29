using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Services;
using LikeBotVK.Domain.Jobs.Entities;
using LikeBotVK.Domain.Jobs.Exceptions;
using LikeBotVK.Domain.VK.Exceptions;
using Type = LikeBotVK.Domain.Jobs.Enums.Type;

namespace LikeBotVK.Domain.Services.Services;

public class JobProcessorService : IJobProcessorService
{
    private readonly IJobFunctionsService _functionsService;
    private readonly IUnitOfWork _unitOfWork;

    public JobProcessorService(IJobFunctionsService functionsService, IUnitOfWork unitOfWork)
    {
        _functionsService = functionsService;
        _unitOfWork = unitOfWork;
    }

    public async Task ProcessJobAsync(Job job, CancellationToken token)
    {
        var vk = await _unitOfWork.VkRepository.Value.GetAsync(job.VkId);
        if (vk == null) throw new JobVkNotFoundException(job);
        if (string.IsNullOrEmpty(vk.AccessToken)) throw new VkNotActiveException(vk);
        if (job.Publications == null || !job.Publications.Any()) throw new PublicationsCollectionEmptyException(job);
        job.MarkAsStarted();
        await _unitOfWork.JobRepository.Value.UpdateAsync(job);
        var startIndex = job.CountSuccess + job.CountErrors;
        var errors = 0;
        for (var i = startIndex; i < job.Publications.Count; i++)
        {
            await job.Delay(token);
            var task = job.Type switch
            {
                Type.Like => _functionsService.LikeAsync(vk, job.Publications[i]),
                Type.Subscribe => _functionsService.FollowAsync(vk, job.Publications[i]),
                Type.Repost => _functionsService.RepostAsync(vk, job.Publications[i]),
                _ => throw new ArgumentOutOfRangeException()
            };

            try
            {
                await task;
                job.UpdateInfo(job.CountErrors, job.CountSuccess + 1);
                errors = 0;
            }
            catch (Exception ex)
            {
                job.UpdateInfo(job.CountErrors + 1, job.CountSuccess);
                job.ErrorMessage = ex.Message;
                errors++;
            }

            token.ThrowIfCancellationRequested();
            await _unitOfWork.JobRepository.Value.UpdateAsync(job);
            if (errors == 10) throw new TooManyErrorsException(job);
        }
    }
}