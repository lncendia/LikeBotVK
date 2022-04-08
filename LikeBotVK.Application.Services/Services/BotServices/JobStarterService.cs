using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Services;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Application.Services.Services.BotServices;

public class JobStarterService : IJobStarterService
{
    private readonly IJobNotifierService _jobNotifier;
    private readonly IGetPublicationService _publicationsGetterService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDataUnitOfWork _applicationDataUnitOfWork;
    private readonly IJobProcessorService _jobProcessor;
    private readonly IProxySetter _proxySetter;

    public JobStarterService(IJobNotifierService jobNotifier, IGetPublicationService publicationsGetterService,
        IUnitOfWork unitOfWork, IJobProcessorService jobProcessor, IApplicationDataUnitOfWork applicationDataUnitOfWork,
        IProxySetter proxySetter)
    {
        _jobNotifier = jobNotifier;
        _publicationsGetterService = publicationsGetterService;
        _unitOfWork = unitOfWork;
        _jobProcessor = jobProcessor;
        _applicationDataUnitOfWork = applicationDataUnitOfWork;
        _proxySetter = proxySetter;
    }

    public async Task StartJobAsync(int id, CancellationToken token)
    {
        var job = await _unitOfWork.JobRepository.Value.GetAsync(id);
        var jobData = await _applicationDataUnitOfWork.JobDataRepository.Value.GetAsync(id);
        if (job == null || jobData == null || string.IsNullOrEmpty(jobData.Hashtag)) return;
        if (job.CountErrors + job.CountSuccess == 0) await _jobNotifier.NotifyStartAsync(job);
        var vk = await _unitOfWork.VkRepository.Value.GetAsync(job.VkId);
        await CheckProxyAsync(vk!);

        if (!job.Publications.Any())
        {
            try
            {
                var publications = await _publicationsGetterService.GetPublicationsAsync(
                    (await _unitOfWork.VkRepository.Value.GetAsync(job.VkId))!, jobData.Hashtag, job.Type,
                    jobData.DateTimeLimitation, token);
                job.AddPublications(publications);
                await _unitOfWork.JobRepository.Value.UpdateAsync(job);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                job.ErrorMessage = ex.Message;
                job.MarkAsCompleted();
                await _unitOfWork.JobRepository.Value.UpdateAsync(job);
                return;
            }
        }

        try
        {
            await _jobProcessor.ProcessJobAsync(job, token);
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            job.ErrorMessage = ex.Message;
        }

        job.MarkAsCompleted();
        await _unitOfWork.JobRepository.Value.UpdateAsync(job);
        await _jobNotifier.NotifyEndAsync(job);
    }

    private async Task CheckProxyAsync(Vk vk)
    {
        if (vk.ProxyId == null)
            if (await _proxySetter.SetProxyAsync(vk))
                await _unitOfWork.VkRepository.Value.UpdateAsync(vk);
    }
}