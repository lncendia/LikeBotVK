using System.ComponentModel.DataAnnotations;

namespace LikeBotVK.Infrastructure.Web.ViewModels.Proxy;

public class ProxySearchViewModel
{
    [Display(Name = "Хост")] public string? Host { get; set; }

    [Range(0, 65536)]
    [Display(Name = "Порт")]
    public int? Port { get; set; }
    public int Page { get; set; } = 1;
}