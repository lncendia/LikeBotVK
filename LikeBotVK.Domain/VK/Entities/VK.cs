using LikeBotVK.Domain.Proxies.Entities;

namespace LikeBotVK.Domain.VK.Entities;

public class Vk
{
    public Vk(long userId, string username, string password)
    {
        UserId = userId;
        Username = username;
        Password = password;
    }

    public int Id { get; init; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string? AccessToken { get; set; }
    public long UserId { get; set; }
    public int? ProxyId { get; set; }

    public void SetProxy(Proxy proxy) => ProxyId = proxy.Id;
}