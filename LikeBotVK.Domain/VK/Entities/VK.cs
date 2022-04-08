using LikeBotVK.Domain.Proxies.Entities;

namespace LikeBotVK.Domain.VK.Entities;

public class Vk
{
    public Vk(long userId, string username, string password)
    {
        Username = username;
        Password = password;
        UserId = userId;
    }

    public int Id { get; set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string? AccessToken { get; set; }

    public long UserId { get; }
    public int? ProxyId { get; private set; }

    public void SetProxy(Proxy proxy) => ProxyId = proxy.Id;

    public void ChangeData(string login, string password)
    {
        AccessToken = null;
        Username = login;
        Password = password;
    }

    public bool IsActive() => !string.IsNullOrEmpty(AccessToken);
}