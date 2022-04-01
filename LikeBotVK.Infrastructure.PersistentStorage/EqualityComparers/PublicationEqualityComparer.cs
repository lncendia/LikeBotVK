using LikeBotVK.Infrastructure.PersistentStorage.Models;

namespace LikeBotVK.Infrastructure.PersistentStorage.EqualityComparers;

public class PublicationEqualityComparer : IEqualityComparer<PublicationModel>
{
    public bool Equals(PublicationModel? x, PublicationModel? y)
    {
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        Console.WriteLine(x.OwnerId == y.OwnerId && x.PublicationId == y.PublicationId);
        return x.OwnerId == y.OwnerId && x.PublicationId == y.PublicationId;
    }

    public int GetHashCode(PublicationModel obj)
    {
        return HashCode.Combine(obj.Id, obj.PublicationId, obj.JobModel);
    }
}