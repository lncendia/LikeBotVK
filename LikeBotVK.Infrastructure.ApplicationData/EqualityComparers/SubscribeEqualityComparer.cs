using LikeBotVK.Infrastructure.ApplicationData.Models;

namespace LikeBotVK.Infrastructure.ApplicationData.EqualityComparers;

public class SubscribeEqualityComparer : IEqualityComparer<SubscribeData>
{
    public bool Equals(SubscribeData? x, SubscribeData? y)
    {
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        return x.EndSubscribe == y.EndSubscribe;
    }

    public int GetHashCode(SubscribeData obj)
    {
        return HashCode.Combine(obj.Id, obj.EndSubscribe, obj.UserData);
    }
}