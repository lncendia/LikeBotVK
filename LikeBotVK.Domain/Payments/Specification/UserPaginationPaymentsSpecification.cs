using Ardalis.Specification;

namespace LikeBotVK.Domain.Payments.Specification;

public sealed class UserPaginationPaymentsSpecification : UserPaymentsSpecification
{
    public UserPaginationPaymentsSpecification(long id, int skip, int take) : base(id)
    {
        Query.Skip(skip).Take(take);
    }
}