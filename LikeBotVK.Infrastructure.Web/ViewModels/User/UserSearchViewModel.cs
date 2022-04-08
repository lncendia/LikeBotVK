using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Infrastructure.Web.ViewModels.User;

public class UserSearchViewModel
{
    [Display(Name = "ID")] public long? UserId { get; set; }
    public int Page { get; set; } = 1;
}