using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Infrastructure.Web.ViewModels.User;

public class AddSubscribeViewModel
{
    [Display(Name = "Окончание подписки (в UTC)")]
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    public DateTime EndSubscribe { get; set; } = DateTime.UtcNow.AddDays(30).Date;
}