namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IUnitOfWork
{
    Lazy<IJobRepository> JobRepository { get; set; }
    Lazy<IProxyRepository> ProxyRepository { get; set; }
    Lazy<IUserRepository> UserRepository { get; set; }
    Lazy<IVkRepository> VkRepository { get; set; }
}