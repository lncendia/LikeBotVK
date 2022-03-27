using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.Users.Entities;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IUserRepository : IRepository<User, long>
{
}