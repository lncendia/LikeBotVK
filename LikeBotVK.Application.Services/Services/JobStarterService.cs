using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Services;
using LikeBotVK.Domain.Proxies.Specification;
using LikeBotVK.Domain.VK.Entities;

namespace LikeBotVK.Application.Services.Services;

public class JobStarterService : IJobStarterService
{
    private readonly IJobNotifierService _jobNotifier;
    private readonly IGetPublicationService _publicationsGetterService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDataUnitOfWork _applicationDataUnitOfWork;
    private readonly IJobProcessorService _jobProcessor;

    public JobStarterService(IJobNotifierService jobNotifier, IGetPublicationService publicationsGetterService,
        IUnitOfWork unitOfWork, IJobProcessorService jobProcessor, IApplicationDataUnitOfWork applicationDataUnitOfWork)
    {
        _jobNotifier = jobNotifier;
        _publicationsGetterService = publicationsGetterService;
        _unitOfWork = unitOfWork;
        _jobProcessor = jobProcessor;
        _applicationDataUnitOfWork = applicationDataUnitOfWork;
    }

    public async Task StartJobAsync(int id, CancellationToken token)
    {
        var t1 = _unitOfWork.JobRepository.Value.GetAsync(id);
        var t2 = _applicationDataUnitOfWork.JobDataRepository.Value.GetAsync(id);
        await Task.WhenAll(t1, t2);
        var job = t1.Result;
        var jobData = t2.Result;
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
                job.SetPublications(publications);
                await _unitOfWork.JobRepository.Value.UpdateAsync(job);
            }
            catch (Exception ex)
            {
                job.ErrorMessage = ex.Message;
                job.CompleteJob();
                await _unitOfWork.JobRepository.Value.UpdateAsync(job);
                return;
            }
        }

        try
        {
            await _jobProcessor.ProcessJobAsync(job, token);
        }
        catch (Exception ex)
        {
            job.ErrorMessage = ex.Message;
        }

        job.CompleteJob();
        await _unitOfWork.JobRepository.Value.UpdateAsync(job);
    }

    private async Task CheckProxyAsync(Vk vk)
    {
        if (vk.ProxyId == null)
        {
            var randomProxy = (await _unitOfWork.ProxyRepository.Value.FindAsync(new RandomProxiesSpecification(1)))
                .FirstOrDefault();
            if (randomProxy != null)
            {
                vk.ProxyId = randomProxy.Id;
                await _unitOfWork.VkRepository.Value.UpdateAsync(vk);
            }
        }
    }
}