using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData;
using LikeBotVK.Infrastructure.PersistentStorage;

namespace LikeBotVK.Extensions;

public static class UnitOfWorks
{
    public static void AddUnitOfWorks(this IServiceCollection services, Configuration.Configuration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IApplicationDataUnitOfWork, ApplicationUnitOfWork>();
    }
}