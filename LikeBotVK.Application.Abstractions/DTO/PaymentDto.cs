namespace LikeBotVK.Application.Abstractions.DTO;

public class PaymentDto
{
    public PaymentDto(long userId, decimal cost, DateTime dateTime)
    {
        UserId = userId;
        Cost = cost;
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        PaymentDate = TimeZoneInfo.ConvertTimeFromUtc(dateTime, tz);
    }
    
    public long UserId { get; }
    public DateTime PaymentDate { get; }
    public decimal Cost { get; }
}