using LikeBotVK.Domain.Abstractions.Interfaces;
using LikeBotVK.Domain.Payments.Entities;

namespace LikeBotVK.Domain.Abstractions.Repositories;

public interface IPaymentRepository : IRepository<Payment, string>
{
}