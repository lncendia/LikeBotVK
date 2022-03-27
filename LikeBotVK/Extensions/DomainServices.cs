using LikeBotVK.Domain.Abstractions.Factories;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Services;
using LikeBotVK.Domain.Services.Factories;
using LikeBotVK.Domain.Services.Services;
using LikeBotVK.Infrastructure.JobFunctions.Services;

namespace LikeBotVK.Extensions;

public static class DomainServices
{
    public static void AddDomainServices(this IServiceCollection services, Configuration.Configuration configuration)
    {
        services.AddScoped<IJobFunctionsService, JobFunctionsService>(provider =>
            new JobFunctionsService(provider.GetService<IUnitOfWork>()!, configuration.AntiCaptchaToken));
        services.AddScoped<IJobProcessorService, JobProcessorService>();
        services.AddScoped<IJobFactory, JobFactory>();
        services.AddScoped<IVkFactory, VkFactory>();
    }
}