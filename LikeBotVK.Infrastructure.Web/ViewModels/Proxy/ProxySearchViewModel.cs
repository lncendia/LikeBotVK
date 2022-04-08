using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Infrastructure.Web.ViewModels.Proxy;

public class PaymentSearchViewModel
{
    [Display(Name = "Хост")] public long? UserId { get; set; }
    public int Page { get; set; } = 1;
}