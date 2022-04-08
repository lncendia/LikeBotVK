using System.Linq.Expressions;
using LikeBotVK.Domain.Specifications.Abstractions;
using LikeBotVK.Domain.Users.Entities;
using LikeBotVK.Domain.Users.Specification.Visitor;
using LikeBotVK.Infrastructure.PersistentStorage.Models;

namespace LikeBotVK.Infrastructure.PersistentStorage.Visitors;

public class UserVisitor : BaseVisitor<UserModel, IUserSpecificationVisitor, User>, IUserSpecificationVisitor
{
    protected override Expression<Func<UserModel, bool>> ConvertSpecToExpression(
        ISpecification<User, IUserSpecificationVisitor> spec)
    {
        var visitor = new UserVisitor();
        spec.Accept(visitor);
        return visitor.Expr!;
    }
}