namespace LikeBotVK.Application.Abstractions.ApplicationData;

public class PaymentData
{
    public PaymentData(long userId, decimal cost, DateTime dateTime)
    {
        UserId = userId;
        Cost = cost;
        PaymentDate = dateTime;
    }

    public string Id { get; set; } = null!;
    public long UserId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Cost { get; set; }
}