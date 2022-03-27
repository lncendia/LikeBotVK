namespace LikeBotVK.Infrastructure.PersistentStorage.Models;

public class VkModel
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? AccessToken { get; set; }
    public long UserId { get; set; }
    public UserModel User { get; set; } = null!;
    public int? ProxyId { get; set; }
    public ProxyModel? Proxy { get; set; }

    public List<JobModel> Jobs { get; set; } = new();
}