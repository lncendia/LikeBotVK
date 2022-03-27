using System.ComponentModel.DataAnnotations;
using LikeBotVK.Application.Abstractions.Enums;

namespace LikeBotVK.Infrastructure.Web.ViewModels.User;

public class UserSearchViewModel
{
    [Display(Name = "ID")] public long? UserId { get; set; }
    [Display(Name = "Админ")] public bool IsAdmin { get; set; }
    [Display(Name = "Заблокирован")] public bool IsBanned { get; set; }
    public int Page { get; set; } = 1;
}