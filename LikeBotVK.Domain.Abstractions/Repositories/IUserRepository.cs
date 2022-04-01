using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.Users.Entities;
using LikeBotVK.Domain.Users.Specification.Visitor;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IUserRepository : IRepository<User, long, IUserSpecificationVisitor>
{
}