namespace LikeBotVK.Infrastructure.PersistentStorage.Models;

public class PaymentModel
{
    public string Id { get; set; } = null!;
    public long UserId { get; set; }
    public UserModel? User { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Cost { get; set; }
}