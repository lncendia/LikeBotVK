using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Infrastructure.ApplicationData.Models;

public class JobData
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int JobId { get; set; }

    public string? BackgroundJobId { get; set; }
    public string? Hashtag { get; set; }
    public DateTime? DateTimeLimitation { get; set; }
    public WorkType? WorkType { get; set; }
}