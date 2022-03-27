using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Domain.Payments.Entities;

namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IPaymentService
{
    Task<PaymentData> CreatePayAsync(long id, decimal cost);
    Task<(decimal, DateTime)> GetPaymentData(string billId);
}