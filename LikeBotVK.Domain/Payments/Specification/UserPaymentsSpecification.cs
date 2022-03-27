using Ardalis.Specification;
using LikeBotVK.Domain.Payments.Entities;

namespace LikeBotVK.Domain.Payments.Specification;

public class UserPaymentsSpecification : Specification<Payment>
{
    public UserPaymentsSpecification(long id)
    {
        Query.Where(payment => payment.UserId == id);
    }
}