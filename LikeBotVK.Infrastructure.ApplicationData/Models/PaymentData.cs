using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LikeBotVK.Infrastructure.ApplicationData.Models;

public class PaymentData
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = null!;

    public long UserId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Cost { get; set; }
}