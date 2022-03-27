using LikeBotVK.Infrastructure.PersistentStorage.Models;

namespace LikeBotVK.Infrastructure.PersistentStorage.EqualityComparers;

public class SubscribeEqualityComparer : IEqualityComparer<SubscribeModel>
{
    public bool Equals(SubscribeModel? x, SubscribeModel? y)
    {
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        return x.EndSubscribe == y.EndSubscribe;
    }

    public int GetHashCode(SubscribeModel obj)
    {
        return HashCode.Combine(obj.Id, obj.UserId, obj.User, obj.EndSubscribe);
    }
}