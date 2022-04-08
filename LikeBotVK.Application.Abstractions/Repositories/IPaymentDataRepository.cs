using PaymentData = LikeBotVK.Application.Abstractions.ApplicationData.PaymentData;

namespace LikeBotVK.Application.Abstractions.Repositories;

public interface IPaymentDataRepository : IBaseRepository<PaymentData, string>
{
    Task<List<PaymentData>> GetUserPaymentsAsync(long userId, int? take = null, int? skip = null);
}