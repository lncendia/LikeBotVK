namespace LikeBotVK.Infrastructure.Web.ViewModels.Proxy;

public class ProxyListViewModel
{
    public ProxySearchViewModel? ProxySearchViewModel { get; set; }
    public List<ProxyViewModel>? Proxies { get; set; }
    public int Count { get; set; }
}