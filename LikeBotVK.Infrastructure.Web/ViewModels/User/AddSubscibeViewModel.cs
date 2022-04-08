using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Infrastructure.Web.ViewModels.User;

public class AddSubscribeViewModel
{
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "ID")]
    public long Id { get; set; }
    
    [Display(Name = "Окончание подписки (по МСК)")]
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    public DateTime? EndSubscribe { get; set; }
}