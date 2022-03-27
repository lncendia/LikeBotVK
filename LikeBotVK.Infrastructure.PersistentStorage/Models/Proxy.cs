namespace LikeBotVK.Infrastructure.PersistentStorage.Models;

public class ProxyModel
{
    public int Id { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public List<VkModel> Vks { get; set; } = new();
}