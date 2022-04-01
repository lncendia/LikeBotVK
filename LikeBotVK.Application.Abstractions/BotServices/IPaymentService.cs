using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.BotServices;

public interface IPaymentService
{
    Task<PaymentData> CreatePayAsync(long id, decimal cost);
    Task<(decimal, DateTime)> GetPaymentData(string billId);
}