using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Infrastructure.ApplicationData.Context;
using LikeBotVK.Infrastructure.ApplicationData.Services;

namespace LikeBotVK.Infrastructure.ApplicationData;

public class ApplicationUnitOfWork : IApplicationDataUnitOfWork
{
    public ApplicationUnitOfWork(ApplicationDbContext context)
    {
        JobDataRepository = new Lazy<IJobDataRepository>(() => new JobDataRepository(context));
        UserDataRepository = new Lazy<IUserDataRepository>(() => new UserDataRepository(context));
    }

    public Lazy<IJobDataRepository> JobDataRepository { get; set; }
    public Lazy<IUserDataRepository> UserDataRepository { get; set; }
}