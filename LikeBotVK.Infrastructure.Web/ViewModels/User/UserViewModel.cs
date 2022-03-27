using System.ComponentModel.DataAnnotations;
using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Infrastructure.Web.ViewModels.User;

public class UserViewModel
{
    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "ID")]
    public long Id { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Состояние")]
    public State State { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Админ")]
    public bool IsAdmin { get; set; }

    [Required(ErrorMessage = "Поле не должно быть пустым")]
    [Display(Name = "Заблокирован")]
    public bool IsBanned { get; set; }

    public List<DateTime> Subscribes { get; set; } = new();
}