using LikeBotVK.Domain.Users.ValueObjects;

namespace LikeBotVK.Domain.Users.Entities;

public class User
{
    public long Id { get; set; }
    public List<Subscribe> Subscribes { get; set; } = new();

    public void AddSubscribe(Subscribe subscribe) => Subscribes.Add(subscribe);
    public void RemoveSubscribe(Subscribe subscribe) => Subscribes.Remove(subscribe);
    public List<Subscribe> GetExpiredSubscribes() => Subscribes.Where(s => s.EndSubscribe <= DateTime.UtcNow).ToList();
}