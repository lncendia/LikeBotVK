using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Repositories;
using LikeBotVK.Application.Abstractions.Services.WebServices;
using PaymentData = LikeBotVK.Application.Abstractions.ApplicationData.PaymentData;

namespace LikeBotVK.Application.Services.Services.WebServices;

public class PaymentService : IPaymentService
{
    private readonly IApplicationDataUnitOfWork _applicationDataUnitOfWork;

    public PaymentService(IApplicationDataUnitOfWork applicationDataUnitOfWork) =>
        _applicationDataUnitOfWork = applicationDataUnitOfWork;

    public async Task<(List<PaymentDto>, int)> FindAsync(long? userId, int page)
    {
        List<PaymentData> data;
        int count;
        if (!userId.HasValue)
        {
            data = (await _applicationDataUnitOfWork.PaymentDataRepository.Value.FindAsync((page - 1) * 30, 30))
                .ToList();
            count = await _applicationDataUnitOfWork.PaymentDataRepository.Value.CountAsync();
        }
        else
        {
            var userPayments =
                await _applicationDataUnitOfWork.PaymentDataRepository.Value.GetUserPaymentsAsync(userId.Value);
            data = userPayments.Skip((page - 1) * 30).Take(30).ToList();
            count = userPayments.Count;
        }

        return (data.Select(x => new PaymentDto(x.UserId, x.Cost, x.PaymentDate)).ToList(), count);
    }
}