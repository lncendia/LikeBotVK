using LikeBotVK.Application.Abstractions.Services.BotServices;
using LikeBotVK.Application.Abstractions.Services.WebServices;
using LikeBotVK.Application.Services.Services.BotServices;
using LikeBotVK.Application.Services.Services.WebServices;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Infrastructure.JobScheduler.Services;
using LikeBotVK.Infrastructure.PublicationsGetter.Services;
using LikeBotVK.Infrastructure.VkAuthentication.Services;
using PaymentService = LikeBotVK.PaymentSystem.Services.PaymentService;

namespace LikeBotVK.Extensions;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection services,
        Configuration.Configuration configuration)
    {
        services.AddScoped<IProxyService, ProxyService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPaymentService, LikeBotVK.Application.Services.Services.WebServices.PaymentService>();

        services.AddScoped<IGetPublicationService, GetPublicationService>(
            provider => new GetPublicationService(provider.GetService<IUnitOfWork>()!,
                configuration.AntiCaptchaToken));
        services.AddScoped<IJobNotifierService, JobNotifierService>();
        services.AddScoped<IJobStarterService, JobStarterService>();
        services.AddScoped<IJobScheduler, JobScheduler>();
        services.AddScoped<IPaymentCreatorService, PaymentService>(
            _ => new PaymentService(configuration.PaymentConfiguration.QiwiToken));
        services.AddScoped<ISubscribeDeleter, SubscribeDeleter>();
        services.AddScoped<IProxySetter, RandomProxySetter>();
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        services.AddScoped<IVkLoginService, VkLoginService>(
            provider => new VkLoginService(provider.GetService<IUnitOfWork>()!,
                configuration.AntiCaptchaToken));
        services.AddScoped<IWorkDeleter, WorkDeleter>();

        var projects =
            configuration.BotConfiguration.Projects.Select(x =>
                new LikeBotVK.Application.Abstractions.Configuration.Project(x.Link, x.Name)).ToList();

        var applicationConfig = new LikeBotVK.Application.Abstractions.Configuration.Configuration(
            configuration.BotConfiguration.HelpAddress, configuration.BotConfiguration.InstructionAddress,
            configuration.PaymentConfiguration.SubscribeCost, configuration.PaymentConfiguration.ReferralBonus,
            configuration.PaymentConfiguration.SubscribeDuration, projects);

        services.AddSingleton(applicationConfig);
    }
}