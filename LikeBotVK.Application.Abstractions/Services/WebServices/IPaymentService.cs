using LikeBotVK.Application.Abstractions.DTO;

namespace LikeBotVK.Application.Abstractions.Services.WebServices;

public interface IPaymentService
{
    Task<(List<PaymentDto>, int)> FindAsync(long? userId, int page);
}