using LikeBotVK.Application.Abstractions.Configuration;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Domain.Abstractions.Repositories;

namespace LikeBotVK.Application.Services.Services.BotServices;

public class ServiceFacade
{
    public readonly IUnitOfWork UnitOfWork;
    public readonly IApplicationDataUnitOfWork ApplicationDataUnitOfWork;
    public readonly IVkLoginService VkLoginService;
    public readonly IPaymentCreatorService PaymentService;
    public readonly IJobScheduler JobScheduler;
    public readonly IProxySetter ProxySetter;
    public readonly Configuration Configuration;

    public ServiceFacade(IVkLoginService vkLoginService, Configuration configuration,
        IPaymentCreatorService paymentService, IJobScheduler jobScheduler, IUnitOfWork unitOfWork,
        IApplicationDataUnitOfWork applicationDataUnitOfWork, IProxySetter proxySetter)
    {
        VkLoginService = vkLoginService;
        Configuration = configuration;
        PaymentService = paymentService;
        JobScheduler = jobScheduler;
        UnitOfWork = unitOfWork;
        ApplicationDataUnitOfWork = applicationDataUnitOfWork;
        ProxySetter = proxySetter;
    }
}