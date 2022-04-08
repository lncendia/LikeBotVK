namespace LikeBotVK.Infrastructure.Web.ViewModels.User;

public class UserListViewModel
{
    public UserSearchViewModel? UserSearchViewModel { get; set; }
    public List<UserLiteViewModel>? Users { get; set; }
    public int Count { get; set; }
}