using LikeBotVK.Domain.Abstractions.Repositories;
using LikeBotVK.Infrastructure.PersistentStorage.Context;
using LikeBotVK.Infrastructure.PersistentStorage.Repositories;

namespace LikeBotVK.Infrastructure.PersistentStorage;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext context)
    {
        JobRepository = new Lazy<IJobRepository>(() => new JobRepository(context));
        PaymentRepository = new Lazy<IPaymentRepository>(() => new PaymentRepository(context));
        ProxyRepository = new Lazy<IProxyRepository>(() => new ProxyRepository(context));
        UserRepository = new Lazy<IUserRepository>(() => new UserRepository(context));
        VkRepository = new Lazy<IVkRepository>(() => new VkRepository(context));
    }

    public Lazy<IJobRepository> JobRepository { get; set; }
    public Lazy<IPaymentRepository> PaymentRepository { get; set; }
    public Lazy<IProxyRepository> ProxyRepository { get; set; }
    public Lazy<IUserRepository> UserRepository { get; set; }
    public Lazy<IVkRepository> VkRepository { get; set; }
}