using LikeBotVK.Infrastructure.Web.ViewModels.Proxy;

namespace LikeBotVK.Infrastructure.Web.ViewModels.Payment;

public class PaymentListViewModel
{
    public PaymentSearchViewModel? PaymentSearchViewModel { get; set; }
    public List<PaymentViewModel>? Payments { get; set; }
    public int Count { get; set; }
}