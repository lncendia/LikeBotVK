using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Application.Abstractions.Configuration;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Factories;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Services;

namespace LikeBotVK.Application.Services;

public class ServiceFacade
{
    public readonly IUnitOfWork UnitOfWork;
    public readonly IApplicationDataUnitOfWork ApplicationDataUnitOfWork;
    public readonly IVkLoginService VkLoginService;
    public readonly IPaymentService PaymentService;
    public readonly IJobScheduler JobScheduler;
    public readonly Configuration Configuration;
    public readonly IUserJobService UserJobService;
    public readonly IVkFactory VkFactory;
    public readonly IJobFactory JobFactory;

    public ServiceFacade(IVkLoginService vkLoginService, Configuration configuration,
        IPaymentService paymentService, IJobScheduler jobScheduler, IUnitOfWork unitOfWork,
        IApplicationDataUnitOfWork applicationDataUnitOfWork, IUserJobService jobService, IVkFactory vkFactory,
        IJobFactory jobFactory)
    {
        VkLoginService = vkLoginService;
        Configuration = configuration;
        PaymentService = paymentService;
        JobScheduler = jobScheduler;
        UnitOfWork = unitOfWork;
        ApplicationDataUnitOfWork = applicationDataUnitOfWork;
        UserJobService = jobService;
        VkFactory = vkFactory;
        JobFactory = jobFactory;
    }
}