using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.Services.BotServices;

public interface IPaymentCreatorService
{
    Task<PaymentData> CreatePayAsync(long id, decimal cost);
    Task<(decimal, DateTime)> GetPaymentData(string billId);
}