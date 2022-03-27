using LikeBotVK.Application.Abstractions.BotServices;
using LikeBotVK.Application.Abstractions.WebServices;
using LikeBotVK.Application.Services.Services;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Infrastructure.JobScheduler.Services;
using LikeBotVK.Infrastructure.PublicationsGetter.Services;
using LikeBotVK.Infrastructure.VkAuthentication.Services;
using LikeBotVK.PaymentSystem.Services;

namespace LikeBotVK.Extensions;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection services,
        Configuration.Configuration configuration)
    {
        services.AddScoped<IProxyParser, ProxyParser>();
        services.AddScoped<IProxyService, ProxyService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGetPublicationService, GetPublicationService>(
            provider => new GetPublicationService(provider.GetService<IUnitOfWork>()!,
                configuration.AntiCaptchaToken));
        services.AddScoped<IJobNotifierService, JobNotifierService>();
        services.AddScoped<IJobStarterService, JobStarterService>();
        services.AddScoped<IJobScheduler, JobScheduler>();
        services.AddScoped<IPaymentService, PaymentService>(
            _ => new PaymentService(configuration.PaymentConfiguration.QiwiToken));
        services.AddScoped<ISubscribeDeleter, SubscribeDeleter>();
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        services.AddScoped<IUserJobService, UserJobService>();
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