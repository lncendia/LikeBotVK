namespace LikeBotVK.Infrastructure.ApplicationData.Models;

public class SubscribeData
{
    public int Id { get; set; }
    public UserData UserData { get; set; } = null!;
    public DateTime EndSubscribe { get; set; }
}