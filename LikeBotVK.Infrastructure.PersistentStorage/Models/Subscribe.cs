namespace LikeBotVK.Infrastructure.PersistentStorage.Models;

public class SubscribeModel
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public UserModel User { get; set; } = null!;
    public DateTime EndSubscribe { get; set; } = DateTime.UtcNow.AddDays(30);
}