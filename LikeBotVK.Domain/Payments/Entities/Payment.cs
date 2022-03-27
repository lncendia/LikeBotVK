using LikeBotVK.Domain.Users.Entities;

namespace LikeBotVK.Domain.Payments.Entities;

public class Payment
{
    public Payment(long userId, decimal cost, DateTime dateTime)
    {
        UserId = userId;
        Cost = cost;
        PaymentDate = dateTime;
    }

    public string Id { get; set; } = null!;
    public long? UserId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Cost { get; set; }
}