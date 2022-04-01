using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.Users.Entities;

namespace LikeBotVK.Domain.Users.Specification.Visitor;

public interface IUserSpecificationVisitor : ISpecificationVisitor<IUserSpecificationVisitor, User>
{
}