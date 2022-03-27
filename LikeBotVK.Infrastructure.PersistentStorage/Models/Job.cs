namespace LikeBotVK.Infrastructure.PersistentStorage.Models;

public class JobModel
{
    public int Id { get; set; }
    public int VkId { get; set; }
    public VkModel Vk { get; set; } = null!;
    public List<PublicationModel> Publications { get; set; } = new();
    public int UpperInterval { get; set; }
    public int LowerInterval { get; set; }
    public LikeBotVK.Domain.Jobs.Enums.Type Type { get; set; }
    public DateTime? StartTime { get; set; }
    public bool IsCompleted { get; set; }

    public string? ErrorMessage { get; set; }
    public int CountErrors { get; set; }
    public int CountSuccess { get; set; }
}