namespace LikeBotVK.Infrastructure.PersistentStorage.Models;

public class PublicationModel
{
    public int Id { get; set; }
    public long PublicationId { get; set; }
    public long OwnerId { get; set; }
    public JobModel JobModel { get; set; } = null!;
}