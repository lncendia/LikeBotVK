namespace LikeBotVK.Application.Abstractions.ApplicationData;

public class PaymentData
{
    public PaymentData(string id, long userId, decimal cost, DateTime dateTime)
    {
        Id = id;
        UserId = userId;
        Cost = cost;
        PaymentDate = dateTime;
    }

    public string Id { get; }
    public long UserId { get; }
    public DateTime PaymentDate { get; }
    public decimal Cost { get; }
}