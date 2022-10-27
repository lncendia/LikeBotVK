using System.Net;
using LikeBotVK.Application.Abstractions.DTO;
using LikeBotVK.Application.Abstractions.Exceptions;
using LikeBotVK.Application.Abstractions.Services.BotServices;
using Newtonsoft.Json;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using Qiwi.BillPayments.Model.Out;
using RestSharp;

namespace LikeBotVK.PaymentSystem.Services;

public class PaymentService : IPaymentCreatorService
{
    private readonly BillPaymentsClient _client;
    private readonly string _qiwiToken;

    public PaymentService(string qiwiToken)
    {
        _qiwiToken = qiwiToken;
        _client = BillPaymentsClientFactory.Create(qiwiToken);
    }

    public async Task<PaymentData> CreatePayAsync(long id, decimal cost)
    {
        try
        {
            var response = await _client.CreateBillAsync(
                new CreateBillInfo
                {
                    BillId = Guid.NewGuid().ToString(),
                    Amount = new MoneyAmount
                    {
                        ValueDecimal = cost,
                        CurrencyEnum = CurrencyEnum.Rub
                    },
                    ExpirationDateTime = DateTime.Now.AddDays(5)
                });

            return new PaymentData(response.BillId, response.PayUrl.ToString(), response.Amount.ValueDecimal);
        }
        catch (Exception ex)
        {
            throw new ErrorCreateBillException(ex.Message, ex);
        }
    }

    public async Task<(decimal, DateTime)> GetPaymentData(string billId)
    {
        try
        {
            RestClient httpClient = new($"https://api.qiwi.com/partner/bill/v1/bills/{billId}");
            var request = new RestRequest();
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {_qiwiToken}");
            var response1 = await httpClient.ExecuteAsync(request);
            if (response1.StatusCode != HttpStatusCode.OK)
                throw new ErrorCheckBillException(response1.StatusDescription ?? "Bad status code", null);
            var response = JsonConvert.DeserializeObject<BillResponse>(response1.Content!);
            if (response?.Status.ValueString != "PAID") throw new BillNotPaidException(billId);
            return (response.Amount.ValueDecimal, response.Status.ChangedDateTime.ToUniversalTime());
        }
        catch (Exception ex) when (ex is not BillNotPaidException)
        {
            throw new ErrorCheckBillException(ex.Message, ex);
        }
    }
}