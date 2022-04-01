using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LikeBotVK.Infrastructure.PersistentStorage.Models;

public class UserModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }
    public List<VkModel> Vks { get; set; } = new();
    public List<PaymentModel> Payments { get; set; } = new();
}