using LikeBotVK.Application.Abstractions.ApplicationData;
using LikeBotVK.Application.Abstractions.DTO;
using PaymentData = LikeBotVK.Application.Abstractions.ApplicationData.PaymentData;

namespace LikeBotVK.Application.Abstractions.Repositories;

public interface IPaymentDataRepository
{
    Task<PaymentData?> GetAsync(string id);
    Task AddOrUpdateAsync(PaymentData data);
    Task DeleteAsync(string id);
    Task<List<PaymentData>> GetUserPaymentsAsync(long userId, int? take = null, int? skip = null);
}