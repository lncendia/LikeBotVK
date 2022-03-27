namespace LikeBotVK.Domain.Users.ValueObjects;

public class Subscribe
{
    public Subscribe(DateTime endSubscribe)
    {
        EndSubscribe = endSubscribe;
    }

    public DateTime EndSubscribe { get; set; }
    public bool IsExpired() => EndSubscribe < DateTime.UtcNow;
}